using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Running_Tracker.Persistence
{
    class RunningTrackerDataAccess : IRunningTrackerDataAccess
    {
        public RunningTrackerDataAccess()
        {

        }
        
        public List<RunningData> LoadPreviousRunnings()
        {
            throw new NotImplementedException();
        }

        public void SaveRunning(RunningData running)
        {
            throw new NotImplementedException();
        }

        public void DeleteRunning(RunningData running)
        {
            throw new NotImplementedException();
        }
        
        public PersonalDatas CurrentPersonalDatas
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public WarningValues CurrentWarningValues
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}