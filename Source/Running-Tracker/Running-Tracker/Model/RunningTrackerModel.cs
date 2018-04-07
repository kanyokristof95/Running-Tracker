using Android.Locations;
using Running_Tracker.Persistence;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Running_Tracker.Model
{
    /// <summary>
    /// The model of the Running Tracker App
    /// </summary>
    public class RunningTrackerModel
    {
        #region Fields

        private readonly IRunningTrackerDataAccess _runningTrackerDataAccess;
        private RunningData _runningData;
        
        private readonly Timer _runningTimer;

        private RunningSpeed _previousRunningSpeed;
        private int _remaningSameColor;
        private int _remaningCalibratingLocation;

        private bool _distanceWarningYet;
        private bool _timeWarningYet;

        private int _speedWarningFrequency;

        private int _numOfContinuouslyStopsForSignal;

        private int _checkVerticalDistanceRemaining;
        private Location _oldLocation;
        private Location _previousLocation;
        private Location _currentLocation;

        #endregion

        #region Default values

        /// <summary>
        /// The minimum time between 2 gps signal.
        /// </summary>
        public int GpsMinTime => 1000;

        /// <summary>
        /// The minimum distance between 2 gps signal.
        /// </summary>
        public int GpsMinDistance => 0;
        
        /// <summary>
        /// Mode of application's running.
        /// </summary>
        private const bool Debug = false;

        /// <summary>
        /// The number of position calibrating.
        /// </summary>
        private const int DefaultCalibratingPosition = (Debug) ? 1 : 7; 

        /// <summary>
        /// The minimum accuracy of gps signal.
        /// </summary>
        public const double DefaultGpsAccuracy = (Debug) ? 60 : 6;
        
        /// <summary>
        /// Minimum gps signal between 2 speed warning.
        /// </summary>
        public const int SpeedWarningFrequency = 10;
        
        /// <summary>
        /// Minimum gps signal to send the location as a stopping location.
        /// </summary>
        public const int NumOfContinuouslyStopsForSignal = 5;

        /// <summary>
        /// Minimus gps signal to calculate vertical distance.
        /// </summary>
        public const int NumOfCheckVerticalDistance = 30;

        /// <summary>
        /// Minimum length of same color route.
        /// </summary>
        public const int MinLengthOfSameColor = 5;

        #endregion

        #region Constructor

        public RunningTrackerModel(IRunningTrackerDataAccess runningTrackerDataAccess = null)
        {
            this._runningTrackerDataAccess = runningTrackerDataAccess;
            _runningData = null;

            _runningTimer = new Timer
            {
                Interval = 1000
            };
            _runningTimer.Elapsed += RunningTime_Elapsed;

            _remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        #endregion

        #region Private Methods

        private void RunningTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_runningData != null)
                OnCurrentRunningDuration(_runningData.Duration);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Change the user's location
        /// </summary>
        /// <param name="location">New location</param>
        public void ChangeLocation(Location location)
        {
            if(_remaningCalibratingLocation > 0)
            {
                // Calibrating the gps
                if (location.Accuracy <= DefaultGpsAccuracy)
                    _remaningCalibratingLocation--;

                if (_remaningCalibratingLocation == 0)
                    OnGPS_Ready(new LocationData(location.Longitude, location.Latitude));
            } else
            {
                // The gps is calibrated

                if (location.Accuracy > DefaultGpsAccuracy)
                    return;
                
                if (_runningData == null)
                {
                    // The running is not started
                    if(location.Speed > 0)
                    {
                        OnUserPosition(new LocationData(location.Longitude, location.Latitude));
                        OnCameraPosition(new LocationData(location.Longitude, location.Latitude));
                    }
                    return;
                }

                // Reduce the warning frequencies
                if (_speedWarningFrequency > 0)
                    _speedWarningFrequency--;


                if (location.Speed == 0)
                {
                    if (_numOfContinuouslyStopsForSignal >= 0)
                    {
                        // If the var was 0 that will be -1, so the signal won't be sent again
                        _numOfContinuouslyStopsForSignal--;
                    }

                    if (_numOfContinuouslyStopsForSignal == 0)
                    {
                        bool near = false;
                        foreach(Position coordinate in _runningData.Stops)
                        {
                            near = location.DistanceTo(new Location("gps") { Longitude = coordinate.Longitude, Latitude = coordinate.Latitude}) < 3 * 14;
                        }

                        if(!near)
                        {
                            OnUserStopped(new LocationData(location.Longitude, location.Latitude, 0, 0, 0, 0, RunningSpeed.Slow));
                            _runningData.AddStop(new Position(location.Latitude, location.Longitude));
                        }
                    }
                }
                else
                {
                    _numOfContinuouslyStopsForSignal = NumOfContinuouslyStopsForSignal;
                }

                // If the user's position is not changed do nothing 
                if (_previousLocation != null && location.Speed == 0) return;

                if(_checkVerticalDistanceRemaining == 0)
                    _oldLocation = location;

                _checkVerticalDistanceRemaining++;

                _previousLocation = _currentLocation;
                _currentLocation = location;

                double distance = 0;
                double verticalDistance = 0;

                if (_previousLocation != null)
                {
                    distance = _currentLocation.DistanceTo(_previousLocation);
                    
                    if (_checkVerticalDistanceRemaining % NumOfCheckVerticalDistance == 0)
                    {
                        verticalDistance = _currentLocation.Altitude - _oldLocation.Altitude;
                        _checkVerticalDistanceRemaining = 0;
                    }
                }

                double up = 0;
                double down = 0;

                if (verticalDistance > 0)
                {
                    up = verticalDistance;
                } else
                {
                    down = -verticalDistance;
                }

                double speed = location.Speed * 3.6;

                RunningSpeed runningSpeed;

                if (_remaningSameColor == 0)
                {
                    _remaningSameColor = MinLengthOfSameColor;

                    if (_previousLocation == null)
                    {
                        runningSpeed = RunningSpeed.StartPoint;
                    }
                    else if (speed < CurrentWarningValues.MinimumSpeed)
                    {
                        runningSpeed = RunningSpeed.Slow;
                    }
                    else if (speed > CurrentWarningValues.MaximumSpeed)
                    {
                        runningSpeed = RunningSpeed.Fast;
                    }
                    else
                    {
                        runningSpeed = RunningSpeed.Normal;
                    }
                }
                else
                {
                    _remaningSameColor--;
                    runningSpeed = _previousRunningSpeed;
                }

                _previousRunningSpeed = runningSpeed;

                LocationData currentLocationData = new LocationData(location.Longitude, location.Latitude, speed, distance, up, down, runningSpeed);
                _runningData.AddLocation(currentLocationData);
                    
                OnNewPosition(currentLocationData);
                    
                // Send warning signals
                if (!_timeWarningYet && _runningData.Duration > CurrentWarningValues.Time)
                {
                    _timeWarningYet = true;
                    OnWarning(WarningType.Time);
                }

                if(!_distanceWarningYet && _runningData.Distance > CurrentWarningValues.Distance)
                {
                    _distanceWarningYet = true;
                    OnWarning(WarningType.Distance);
                }
                    
                if (_speedWarningFrequency == 0)
                {
                    if (_currentLocation.Speed * 3.6 < CurrentWarningValues.MinimumSpeed)
                    {
                        _speedWarningFrequency = SpeedWarningFrequency;
                        OnWarning(WarningType.SpeedSlow);
                    }

                    if (_currentLocation.Speed * 3.6 > CurrentWarningValues.MaximumSpeed)
                    {
                        _speedWarningFrequency = SpeedWarningFrequency;
                        OnWarning(WarningType.SpeedFast);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Calibrate the gps
        /// </summary>
        public void Calibrate()
        {
            _remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        /// <summary>
        /// Start a new running
        /// </summary>
        public void StartRunning()
        {
            if (_remaningCalibratingLocation == 0)
            {
                _runningData = new RunningData(CurrentPersonalDatas);
                _speedWarningFrequency = SpeedWarningFrequency;
                _distanceWarningYet = false;
                _timeWarningYet = false;
                _numOfContinuouslyStopsForSignal = NumOfContinuouslyStopsForSignal;
                _checkVerticalDistanceRemaining = 0;

                _previousRunningSpeed = RunningSpeed.Normal;
                _remaningSameColor = 0;

                _runningTimer.Start();
            } else
            {
                throw new gpsNotReadyException();
            }
        }
        
        /// <summary>
        /// Stop and save the current running.
        /// </summary>
        public bool StopRunning()
        {
            if(_runningData != null)
            {
                _runningData.Finish();
                _runningTimer.Stop();
                bool ret = SaveRunning(_runningData);
                _runningData = null;
                return ret;
            }
            return false;
        }

        public void SaveSettings(PersonalData personalData, WarningValues warningValues)
        {
            _runningTrackerDataAccess.CurrentPersonalDatas = personalData;
            _runningTrackerDataAccess.CurrentWarningValues = warningValues;
        }
        
        #endregion

        #region Events
        /// <summary>
        /// This signal is sent when the gps configuration is ready.
        /// </summary>
        public event EventHandler<PositionArgs> GpsReady;

        /// <summary>
        /// This signal is sent when a new position was processed and saved by the model.
        /// </summary>
        public event EventHandler<PositionArgs> NewPosition;

        /// <summary>
        /// This signal is sent when the running is not started. It contains the user's latest position.
        /// </summary>
        public event EventHandler<PositionArgs> UserPosition;

        /// <summary>
        /// This signal is sent when the user stopped.
        /// </summary>
        public event EventHandler<PositionArgs> UserStopped;

        /// <summary>
        /// This signal contains the new camera position
        /// </summary>
        public event EventHandler<PositionArgs> CameraPosition;

        /// <summary>
        /// This signal is sent continously during the running. It contains the duration of current running.
        /// </summary>
        public event EventHandler<TimeSpanArgs> CurrentRunningDuration;

        /// <summary>
        /// This signal is sent when a warning was occured.
        /// </summary>
        public event EventHandler<WarningArgs> Warning;


        private void OnNewPosition(LocationData location)
        {
            NewPosition?.Invoke(this, new PositionArgs(location));
        }

        private void OnUserPosition(LocationData location)
        {
            UserPosition?.Invoke(this, new PositionArgs(location));
        }

        private void OnUserStopped(LocationData location)
        {
            UserStopped?.Invoke(this, new PositionArgs(location));
        }

        private void OnCameraPosition(LocationData location)
        {
            CameraPosition?.Invoke(this, new PositionArgs(location));
        }

        private void OnCurrentRunningDuration(TimeSpan timeSpan)
        {
            CurrentRunningDuration?.Invoke(this, new TimeSpanArgs(timeSpan));
        }

        private void OnWarning(WarningType warning)
        {
            Warning?.Invoke(this, new WarningArgs(warning));
        }
        
        private void OnGPS_Ready(LocationData location)
        {
            GpsReady?.Invoke(this, new PositionArgs(location));
        }

        #endregion

        #region Persistence Methods

        /// <summary>
        /// Returns the current running's locations.
        /// </summary>
        public List<LocationData> Locations => _runningData.Locations;

        /// <summary>
        /// Returns the previous saved runnings.
        /// </summary>
        public List<RunningData> LoadPreviousRunnings()
        {
            return _runningTrackerDataAccess.LoadPreviousRunnings();
        }

        /// <summary>
        /// Save the running in the parameter.
        /// </summary>
        public bool SaveRunning(RunningData running)
        {
            if (running.Locations.Count < 2) return false;
            _runningTrackerDataAccess.SaveRunning(running);
            return true;
        }

        /// <summary>
        /// Delete the running in the parameter.
        /// </summary>
        public void DeleteRunning(RunningData running)
        {
            _runningTrackerDataAccess.DeleteRunning(running);
        }

        /// <summary>
        /// Property for the current personal datas.
        /// </summary>
        public PersonalData CurrentPersonalDatas
        {
            get => _runningTrackerDataAccess.CurrentPersonalDatas;
            set => _runningTrackerDataAccess.CurrentPersonalDatas = value;
        }

        /// <summary>
        /// Property for the current warning values.
        /// </summary>s
        public WarningValues CurrentWarningValues
        {
            get => _runningTrackerDataAccess.CurrentWarningValues;
            set => _runningTrackerDataAccess.CurrentWarningValues = value;
        }

        /// <summary>
        /// If the user starts the running, it return true
        /// </summary>
        public bool IsRunning => _runningData != null;

        #endregion
    }
}