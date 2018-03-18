using Android.Gms.Maps.Model;
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

        private IRunningTrackerDataAccess runningTrackerDataAccess;
        private RunningData runningData;
        
        private Timer runningTimer;

        private int remaningCalibratingLocation;

        private bool distanceWarningYet;
        private bool timeWarningYet;

        private int speedWarningFrequency;

        private int numOfContinuouslyStopsForSignal;

        private Location previousLocation;
        private Location currentLocation;

        #endregion

        #region Default values

        /// <summary>
        /// The minimum time between 2 gps signal.
        /// </summary>
        public int GPSMinTime
        {
            get { return 500; } // TODO - maybe to 1000
        }

        /// <summary>
        /// The minimum distance between 2 gps signal.
        /// </summary>
        public int GPSMinDistance
        {
            get { return 0; }
        }


        public const bool debug = false;

        /// <summary>
        /// The number of position calibrating.
        /// </summary>
        public const int DefaultCalibratingPosition = (debug) ? 1 : 8; // TODO - maybe to 10

        /// <summary>
        /// The minimum accuracy of gps signal.
        /// </summary>
        public const double DefaultGpsAccuracy = (debug) ? 60 : 6;
        
        /// <summary>
        /// Minimum gps signal between 2 speed warning
        /// </summary>
        public const int SpeedWarningFrequency = 10;
        
        /// <summary>
        /// Minimum gps signal to send the location as a stopping location
        /// </summary>
        public const int NumOfContinuouslyStopsForSignal = 8;

        #endregion

        #region Constructor

        public RunningTrackerModel(IRunningTrackerDataAccess runningTrackerDataAccess = null)
        {
            this.runningTrackerDataAccess = runningTrackerDataAccess;
            runningData = null;

            runningTimer = new Timer
            {
                Interval = 1000
            };
            runningTimer.Elapsed += RunningTime_Elapsed;

            remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        #endregion

        #region Private Methods

        private void RunningTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (runningData != null)
                OnCurrentRunningDuration(runningData.Duration);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Change the user's location
        /// </summary>
        /// <param name="location">New location</param>
        public void ChangeLocation(Location location)
        {
            if(remaningCalibratingLocation > 0)
            {
                // Calibrating the gps
                if (location.Accuracy <= DefaultGpsAccuracy)
                    remaningCalibratingLocation--;

                if (remaningCalibratingLocation == 0)
                    OnGPS_Ready(new LocationData(location.Longitude, location.Latitude));
            } else
            {
                // The gps is calibrated

                if (location.Accuracy > DefaultGpsAccuracy)
                    return;
                
                if (runningData == null)
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
                if (speedWarningFrequency > 0)
                    speedWarningFrequency--;


                if (location.Speed == 0)
                {
                    if (numOfContinuouslyStopsForSignal >= 0)
                    {
                        // If the var was 0 that will be -1, so the signal won't be sent again
                        numOfContinuouslyStopsForSignal--;
                    }

                    if (numOfContinuouslyStopsForSignal == 0)
                    {
                        OnUserStopped(new LocationData(location.Longitude, location.Latitude, 0, 0, 0, 0, RunningSpeed.Slow));
                        runningData.AddStop(new LatLng(location.Latitude, location.Longitude));
                    }
                }
                else
                {
                    numOfContinuouslyStopsForSignal = NumOfContinuouslyStopsForSignal;
                }

                // If the user's position is not changed do nothing 
                if (previousLocation != null && location.Speed == 0) return;
                
                previousLocation = currentLocation;
                currentLocation = location;

                double distance = 0;
                double verticalDistance = 0;

                if (previousLocation != null)
                {
                    distance = currentLocation.DistanceTo(previousLocation);
                    verticalDistance = currentLocation.Altitude - previousLocation.Altitude;
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
                if (previousLocation == null)
                {
                    runningSpeed = RunningSpeed.StartPoint;
                } else if (speed < CurrentWarningValues.MinimumSpeed)
                {
                    runningSpeed = RunningSpeed.Slow;
                } else if (speed > CurrentWarningValues.MaximumSpeed)
                {
                    runningSpeed = RunningSpeed.Fast;
                } else
                {
                    runningSpeed = RunningSpeed.Normal;
                }
                    
                LocationData currentLocationData = new LocationData(location.Longitude, location.Latitude, speed, distance, up, down, runningSpeed);
                runningData.AddLocation(currentLocationData);
                    
                OnNewPosition(currentLocationData);
                    
                // Send warning signals
                if (!timeWarningYet && runningData.Duration > CurrentWarningValues.Time)
                {
                    timeWarningYet = true;
                    OnWarning(WarningType.Time);
                }

                if(!distanceWarningYet && runningData.Distance > CurrentWarningValues.Distance)
                {
                    distanceWarningYet = true;
                    OnWarning(WarningType.Distance);
                }
                    
                if (speedWarningFrequency == 0)
                {
                    if (currentLocation.Speed * 3.6 < CurrentWarningValues.MinimumSpeed)
                    {
                        speedWarningFrequency = SpeedWarningFrequency;
                        OnWarning(WarningType.SpeedSlow);
                    }

                    if (currentLocation.Speed * 3.6 > CurrentWarningValues.MaximumSpeed)
                    {
                        speedWarningFrequency = SpeedWarningFrequency;
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
            remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        /// <summary>
        /// Start a new running
        /// </summary>
        public void StartRunning()
        {
            if (remaningCalibratingLocation == 0)
            {
                runningData = new RunningData(CurrentPersonalDatas);
                speedWarningFrequency = SpeedWarningFrequency;
                distanceWarningYet = false;
                timeWarningYet = false;
                numOfContinuouslyStopsForSignal = NumOfContinuouslyStopsForSignal;
                runningTimer.Start();
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
            if(runningData != null)
            {
                runningData.Finish();
                runningTimer.Stop();
                bool ret = SaveRunning(runningData);
                runningData = null;
                return ret;
            }
            return false;
        }
        
        #endregion

        #region Events
        /// <summary>
        /// This signal is sent when the gps configuration is ready.
        /// </summary>
        public event EventHandler<PositionArgs> GPS_Ready;

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
            GPS_Ready?.Invoke(this, new PositionArgs(location));
        }

        #endregion

        #region Persistence Methods

        /// <summary>
        /// Returns the current running's locations.
        /// </summary>
        public List<LocationData> Locations
        {
            get
            {
                return runningData.Locations;
            }
        }

        /// <summary>
        /// Returns the previous saved runnings.
        /// </summary>
        public List<RunningData> LoadPreviousRunnings()
        {
            return runningTrackerDataAccess.LoadPreviousRunnings();
        }

        /// <summary>
        /// Save the running in the parameter.
        /// </summary>
        public bool SaveRunning(RunningData running)
        {
            if (running.Locations.Count >= 2)
            {
                runningTrackerDataAccess.SaveRunning(running);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete the running in the parameter.
        /// </summary>
        public void DeleteRunning(RunningData running)
        {
            runningTrackerDataAccess.DeleteRunning(running);
        }

        /// <summary>
        /// Property for the current personal datas.
        /// </summary>
        public PersonalData CurrentPersonalDatas
        {
            get { return runningTrackerDataAccess.CurrentPersonalDatas; }
            set { runningTrackerDataAccess.CurrentPersonalDatas = value; }
        }

        /// <summary>
        /// Property for the current warning values.
        /// </summary>s
        public WarningValues CurrentWarningValues
        {
            get { return runningTrackerDataAccess.CurrentWarningValues; }
            set { runningTrackerDataAccess.CurrentWarningValues = value; }
        }

        public bool IsRunning
        {
            get { return runningData != null; }
        }

        #endregion
    }
}