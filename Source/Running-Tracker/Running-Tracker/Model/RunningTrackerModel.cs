using System;
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
        private GPS_Status gps_Status;

        #endregion

        #region Constructor

        public RunningTrackerModel(IRunningTrackerDataAccess runningTrackerDataAccess = null)
        {
            this.runningTrackerDataAccess = runningTrackerDataAccess;
            runningData = null;
            gps_Status = GPS_Status.NotWorking;
        }

        #endregion

        #region Public Methods

        public void changeLocation(Location location)
        {
            throw new NotImplementedException();
        }

        public void startRunning()
        {
            throw new NotImplementedException();
        }

        public void stopRunning()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event EventHandler<PositionArgs> NewPosition;
        public event EventHandler<PositionArgs> GPS_Ready;

        private void OnNewPosition(LocationData location)
        {
            throw new NotImplementedException();

            if (GPS_Ready != null)
                GPS_Ready(this, new PositionArgs(location));
        }

        private void OnGPS_Ready(LocationData location)
        {
            throw new NotImplementedException();

            if (GPS_Ready != null)
                GPS_Ready(this, new PositionArgs(location));
        }

        #endregion

        #region Persistence Methods

        public void saveRunning(RunningData running)
        {
            runningTrackerDataAccess.saveRunning(running);
        }

        public void deleteRunning(RunningData running)
        {
            runningTrackerDataAccess.deleteRunning(running);
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