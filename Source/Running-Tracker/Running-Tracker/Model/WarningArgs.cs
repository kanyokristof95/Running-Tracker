using System;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the type of warning: Slow speed, Fast speed, Too much time, Too long distance.
    /// </summary>
    public class WarningArgs : EventArgs
    {
        public WarningArgs(WarningType warningType)
        {
            Warning = warningType;
        }

        public WarningType Warning { get; }
    }
}