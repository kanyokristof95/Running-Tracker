using System.Collections.Generic;

namespace Running_Tracker.Persistence
{
    public interface IRunningTrackerDataAccess
    {
        List<RunningData> LoadPreviousRunnings();

        void SaveRunning(RunningData running);

        void DeleteRunning(RunningData running);

        PersonalDatas CurrentPersonalDatas { get; set; }

        WarningValues CurrentWarningValues { get; set; }
    }
}