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
    
        private int speedWarningFrequency;
        private int distanceWarningFrequency;
        private int timeWarningFrequency;
        private int stopWarningFrequency;
        
        private Location previousLocation;
        private Location currentLocation;

        #endregion

        #region Default values

        /// <summary>
        /// The minimum time between 2 gps signal.
        /// </summary>
        public int GPSMinTime
        {
            get { return 1000; }
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
        public const int DefaultCalibratingPosition = (debug) ? 1 : 10; // TODO 10

        /// <summary>
        /// The minimum accuracy of gps signal.
        /// </summary>
        public const double DefaultGpsAccuracy = (debug) ? 60 : 6; // TODO 6

        /// <summary>
        /// Speed of slow running in km/h
        /// </summary>
        public const double SlowRunning = 8;

        /// <summary>
        /// Speed of fast running in km/h
        /// </summary>
        public const double FastRunning = 16;

        /// <summary>
        /// Minimum gps signal between 2 speed warning
        /// </summary>
        public const int SpeedWarningFrequency = 10;

        /// <summary>
        /// Minimum gps signal between 2 distance warning
        /// </summary>
        public const int DistanceWarningFrequency = 30;

        /// <summary>
        /// Minimum gps signal between 2 time warning
        /// </summary>
        public const int TimeWarningFrequency = 30;

        /// <summary>
        /// Minimum gps signal between 2 stop warning
        /// </summary>
        public const int StopWarningFrequency = 10;

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
                if (location.Accuracy > DefaultGpsAccuracy)
                    return;

                    // The gps is calibrated
                if (runningData == null)
                {
                    // The running is not started
                    OnUserPosition(new LocationData(location.Longitude, location.Latitude));
                    return;
                }

                // Reduce the warning frequencies
                if (stopWarningFrequency > 0)
                    stopWarningFrequency--;

                if (timeWarningFrequency > 0)
                    timeWarningFrequency--;

                if (distanceWarningFrequency > 0)
                    distanceWarningFrequency--;

                if (speedWarningFrequency > 0)
                    speedWarningFrequency--;

                // If the user's position is not changed do nothing 
                if (previousLocation != null && previousLocation.Speed == 0 && location.Speed == 0) return;
                
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
                } else if (speed < SlowRunning)
                {
                    runningSpeed = RunningSpeed.Slow;
                } else if (speed > FastRunning)
                {
                    runningSpeed = RunningSpeed.Fast;
                } else
                {
                    runningSpeed = RunningSpeed.Normal;
                }
                    
                if (stopWarningFrequency == 0 && currentLocation.Speed == 0)
                {
                    stopWarningFrequency = StopWarningFrequency;
                    runningSpeed = RunningSpeed.Stop;
                }
                    
                LocationData currentLocationData = new LocationData(location.Longitude, location.Latitude, speed, distance, up, down, runningSpeed);
                runningData.Add(currentLocationData);
                    
                OnNewPosition(currentLocationData);
                    
                // Send warning signals
                if (timeWarningFrequency == 0)
                {
                    if (runningData.Duration > CurrentWarningValues.Time)
                    {
                        timeWarningFrequency = TimeWarningFrequency;
                        OnWarning(WarningType.Time);
                    }
                }

                if(distanceWarningFrequency == 0)
                {
                    if (runningData.Distance > CurrentWarningValues.Distance)
                    {
                        distanceWarningFrequency = DistanceWarningFrequency;
                        OnWarning(WarningType.Distance);
                    }
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
                distanceWarningFrequency = DistanceWarningFrequency;
                timeWarningFrequency = TimeWarningFrequency;
                stopWarningFrequency = StopWarningFrequency;
                runningTimer.Start();
            } else
            {
                OnGPS_NotReady();
            }
        }
        
        /// <summary>
        /// Stop and save the current running.
        /// </summary>
        public void StopRunning()
        {
            if(runningData != null)
            {
                runningData.Finish();
                runningTimer.Stop();
                SaveRunning(runningData);
                runningData = null;
            }
        }
        
        #endregion

        #region Events
        /// <summary>
        /// This signal is sent when the gps configuration is ready.
        /// </summary>
        public event EventHandler<PositionArgs> GPS_Ready;

        /// <summary>
        /// This signal is sent when the user try to start a new running but the gps is not configured.
        /// </summary>
        public event EventHandler GPS_NotReady;

        /// <summary>
        /// This signal is sent when a new position was processed and saved by the model.
        /// </summary>
        public event EventHandler<PositionArgs> NewPosition;

        /// <summary>
        /// This signal is sent when the running is not started. It contains the user's latest position.
        /// </summary>
        public event EventHandler<PositionArgs> UserPosition;

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

        private void OnCurrentRunningDuration(TimeSpan timeSpan)
        {
            CurrentRunningDuration?.Invoke(this, new TimeSpanArgs(timeSpan));
        }

        private void OnWarning(WarningType warning)
        {
            Warning?.Invoke(this, new WarningArgs(warning));
        }

        private void OnGPS_NotReady()
        {
            GPS_NotReady?.Invoke(this, EventArgs.Empty);
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
        public void SaveRunning(RunningData running)
        {
            if(running.Locations.Count >= 2)
                runningTrackerDataAccess.SaveRunning(running);
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
        public PersonalDatas CurrentPersonalDatas
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

        #endregion
    }
}