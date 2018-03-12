using Android.Locations;
using Running_Tracker.Persistence;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Running_Tracker.Model
{
    public class RunningTrackerModel
    {
        #region Fields

        private IRunningTrackerDataAccess runningTrackerDataAccess;
        private RunningData runningData;
        
        private Timer timer;

        private int remaningCalibratingLocation;

        private Location previousLocation;
        private Location currentLocation;

        #endregion

        #region Properties

        public int GPSMinTime
        {
            get { return 1000; }
        }

        public int GPSMinDistance
        {
            get { return 0; }
        }

        public int DefaultCalibratingPosition
        {
            get { return 10; }
        }

        public double DefaultGpsAccuracy
        {
            get { return 60; } // TODO - to 6
        }

        /// <summary>
        /// km/h
        /// </summary>
        public double SlowRunning
        {
            get { return 8; }
        }

        /// <summary>
        /// km/h
        /// </summary>
        public double FastRunning
        {
            get { return 16; }
        }

        #endregion

        #region Constructor

        public RunningTrackerModel(IRunningTrackerDataAccess runningTrackerDataAccess = null)
        {
            this.runningTrackerDataAccess = runningTrackerDataAccess;
            runningData = null;

            timer = new Timer
            {
                Interval = 1000
            };
            timer.Elapsed += Timer_Elapsed;

            remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (runningData != null)
                OnCurrentTimeSpan(runningData.Time);
        }

        #endregion

        #region Public Methods

        public void ChangeLocation(Location location)
        {
            if(remaningCalibratingLocation > 0)
            {
                // Calibrating
                if (location.Accuracy <= DefaultGpsAccuracy)
                    remaningCalibratingLocation--;

                if (remaningCalibratingLocation == 0)
                    OnGPS_Ready(new LocationData(location.Longitude, location.Latitude));
            } else
            {
                // Working

                if (runningData == null) return;

                if (location.Accuracy <= DefaultGpsAccuracy)
                {
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
                    } else if (speed == 0) {
                        runningSpeed = RunningSpeed.Stop;
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

                    LocationData currentLocationData = new LocationData(location.Longitude, location.Latitude, speed, distance, up, down, runningSpeed);
                    runningData.Add(currentLocationData);
                    
                    OnNewPosition(currentLocationData);

                    // Warning
                    // TODO - Ne küldje folyamatosan a jelet
                    if(runningData.Time > CurrentWarningValues.Time)
                    {
                        OnWarning(WarningType.Time);
                    } else if(runningData.Distance > CurrentWarningValues.Distance)
                    {
                        OnWarning(WarningType.Distance);
                    } else if(currentLocation.Speed == 0)
                    {
                        OnWarning(WarningType.DistanceStop);
                    } else if (currentLocation.Speed * 3.6 < CurrentWarningValues.MinimumSpeed)
                    {
                        OnWarning(WarningType.DistanceSlow);
                    } else if(currentLocation.Speed * 3.6 > CurrentWarningValues.MaximumSpeed)
                    {
                        OnWarning(WarningType.DistanceFast);
                    }
                }
            }
        }

        public void Calibrate()
        {
            remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        public void StartRunning()
        {
            // Küldje el, hogy mióta fut
            if (remaningCalibratingLocation == 0)
            {
                runningData = new RunningData(CurrentPersonalDatas);
                timer.Start();
            } else
            {
                OnGPS_NotReady();
            }
        }

        public void StopRunning()
        {
            if(runningData != null)
            {
                runningData.Finish();
                runningData = null;
                timer.Stop();
                SaveRunning(runningData);
            }
        }

        public List<LocationData> Locations
        {
            get
            {
                return runningData.Locations;
            }
        }

        #endregion

        #region Events

        public event EventHandler<PositionArgs> GPS_Ready;
        public event EventHandler GPS_NotReady;

        public event EventHandler<PositionArgs> NewPosition;
        public event EventHandler<TimeSpanArgs> CurrentTimeSpan;
        public event EventHandler<WarningArgs> Warning;
        

        private void OnNewPosition(LocationData location)
        {
            NewPosition?.Invoke(this, new PositionArgs(location));
        }

        private void OnCurrentTimeSpan(TimeSpan timeSpan)
        {
            CurrentTimeSpan?.Invoke(this, new TimeSpanArgs(timeSpan));
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
        
        public List<RunningData> LoadPreviousRunnings()
        {
            return runningTrackerDataAccess.LoadPreviousRunnings();
        }

        public void SaveRunning(RunningData running)
        {
            runningTrackerDataAccess.SaveRunning(running);
        }

        public void DeleteRunning(RunningData running)
        {
            runningTrackerDataAccess.DeleteRunning(running);
        }

        public PersonalDatas CurrentPersonalDatas
        {
            get { return runningTrackerDataAccess.CurrentPersonalDatas; }
            set { runningTrackerDataAccess.CurrentPersonalDatas = value; }
        }

        public WarningValues CurrentWarningValues
        {
            get { return runningTrackerDataAccess.CurrentWarningValues; }
            set { runningTrackerDataAccess.CurrentWarningValues = value; }
        }

        #endregion
    }
}