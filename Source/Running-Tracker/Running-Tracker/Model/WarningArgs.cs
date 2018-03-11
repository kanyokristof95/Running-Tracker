using System;

namespace Running_Tracker.Model
{
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