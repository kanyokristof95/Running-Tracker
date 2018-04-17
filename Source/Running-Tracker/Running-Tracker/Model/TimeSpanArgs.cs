using System;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the current running time.
    /// </summary>
    public class TimeSpanArgs : EventArgs
    {
        public TimeSpanArgs(TimeSpan timeSpan)
        {
            Time = timeSpan;
        }

        public TimeSpan Time { get; }
    }
}