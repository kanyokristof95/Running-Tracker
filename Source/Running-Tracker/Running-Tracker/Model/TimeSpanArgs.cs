using System;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the current running time.
    /// </summary>
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