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
    public interface IRunningTrackerDataAccess
    {
        List<RunningData> loadPreviousRunnings();

        void saveRunning(RunningData running);

        void deleteRunning(RunningData running);

        PersonalDatas CurrentPersonalDatas { get; set; }

        WarningValues CurrentWarningValues { get; set; }
    }
}