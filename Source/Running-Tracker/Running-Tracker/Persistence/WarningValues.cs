using System;

namespace Running_Tracker.Persistence
{
    public class WarningValues
    {
        /// <summary>
        /// In meter
        /// </summary>
        public double Distance { get; set; }

        public TimeSpan Time { get; set; }

        /// <summary>
        /// In km/h
        /// </summary>
        public double MinimumSpeed { get; set; }

        /// <summary>
        /// In km/h
        /// </summary>
        public double MaximumSpeed { get; set; }
    }
}