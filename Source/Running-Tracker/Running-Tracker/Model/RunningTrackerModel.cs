﻿using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Running_Tracker.Persistence;

namespace Running_Tracker.Model
{
    public class RunningTrackerModel
    {
        #region Fields

        private IRunningTrackerDataAccess runningTrackerDataAccess;
        private RunningData runningData;
        
        private int remaningCalibratingLocation;

        private Location previousLocation;
        private Location currentLocation;

        #endregion

        #region Properties

        public int GPSMinTime
        {
            get { return 3; }
        }

        public int GPSMinDistance
        {
            get { return 0; }
        }

        public int DefaultCalibratingPosition
        {
            get { return 5; }
        }

        public double DefaultGpsAccuracy
        {
            get { return 6; }
        }

        #endregion

        #region Constructor

        public RunningTrackerModel(IRunningTrackerDataAccess runningTrackerDataAccess = null)
        {
            this.runningTrackerDataAccess = runningTrackerDataAccess;
            runningData = null;
            remaningCalibratingLocation = DefaultCalibratingPosition;
        }

        #endregion

        #region Public Methods

        public void ChangeLocation(Location location)
        {
            if (runningData == null) return;

            if(remaningCalibratingLocation > 0)
            {
                // Calibrating
                if (location.Accuracy <= DefaultGpsAccuracy)
                    remaningCalibratingLocation--;

                if (remaningCalibratingLocation == 0)
                    OnGPS_Ready();
            } else
            {
                // Working
                if (location.Accuracy <= DefaultGpsAccuracy)
                {
                    previousLocation = currentLocation;
                    currentLocation = location;

                    double distance = 0;
                    if(previousLocation != null)
                        currentLocation.DistanceTo(previousLocation);

                    double verticalDistance = currentLocation.Altitude - previousLocation.Altitude;

                    double up = 0;
                    double down = 0;

                    if (verticalDistance > 0)
                    {
                        up = verticalDistance;
                    } else
                    {
                        down = -verticalDistance;
                    }

                    LocationData currentLocationData = new LocationData(currentLocation, distance, up, down);
                    runningData.Add(currentLocationData);
                    
                    OnNewPosition(currentLocationData);

                    // Warning

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
            if (remaningCalibratingLocation == 0)
                runningData = new RunningData(CurrentPersonalDatas);
            else
                OnGPS_NotReady();
        }

        public void StopRunning()
        {
            runningData.Finish();
            SaveRunning(runningData);
            runningData = null;
        }

        #endregion

        #region Events

        public event EventHandler GPS_Ready;
        public event EventHandler GPS_NotReady;

        public event EventHandler<PositionArgs> NewPosition;
        public event EventHandler<WarningArgs> Warning;

        private void OnNewPosition(LocationData location)
        {
            NewPosition?.Invoke(this, new PositionArgs(location));
        }

        private void OnWarning(WarningType warning)
        {
            Warning?.Invoke(this, new WarningArgs(warning));
        }

        private void OnGPS_NotReady()
        {
            GPS_NotReady?.Invoke(this, EventArgs.Empty);
        }

        private void OnGPS_Ready()
        {
            GPS_Ready?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Persistence Methods

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