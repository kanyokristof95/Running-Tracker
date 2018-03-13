using System.Collections.Generic;

namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Interface for Running Tracker App data access
    /// </summary>
    public interface IRunningTrackerDataAccess
    {
        /// <summary>
        /// Returns the previous saved runnings.
        /// </summary>
        List<RunningData> LoadPreviousRunnings();

        /// <summary>
        /// Save the running in the parameter.
        /// </summary>
        void SaveRunning(RunningData running);

        /// <summary>
        /// Delete the running in the parameter.
        /// </summary>
        void DeleteRunning(RunningData running);

        /// <summary>
        /// Property for the current personal datas.
        /// </summary>
        PersonalDatas CurrentPersonalDatas { get; set; }

        /// <summary>
        /// Property for the current warning values.
        /// </summary>
        WarningValues CurrentWarningValues { get; set; }
    }
}