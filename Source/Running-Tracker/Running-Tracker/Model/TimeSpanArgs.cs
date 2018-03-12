using System;

namespace Running_Tracker.Model
{
    public class TimeSpanArgs : EventArgs
    {
        private TimeSpan timeSpan;

        public TimeSpanArgs(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public TimeSpan Time
        {
            get { return timeSpan; }
        }
    }
}