using System;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the type of warning: Slow speed, Fast speed, Too much time, Too long distance.
    /// </summary>
    public class WarningArgs : EventArgs
    {
        private WarningType warningType;

        public WarningArgs(WarningType warningType)
        {
            this.warningType = warningType;
        }

        public WarningType Warning
        {
            get { return warningType; }
        }
    }
}