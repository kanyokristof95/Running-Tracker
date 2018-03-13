using System;

namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Contains the warning values
    /// </summary>
    public class WarningValues
    {
        /// <summary>
        /// Warn if the distance of running is more then this property.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Warn if the distance of running is more then this property.
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// In km/h
        /// </summary>
        public double MinimumSpeed { get; set; }

        /// <summary>
        /// In km/h
        /// </summary>
        public double MaximumSpeed { get; set; }

        public WarningValues(double distance = 3000, TimeSpan time = default(TimeSpan), double minimumSpeed = 3, double maximumSpeed = 20)
        {
            if (time == default(TimeSpan))
                time = new TimeSpan(0, 30, 0);

            Distance = distance;
            Time = time;
            MinimumSpeed = minimumSpeed;
            MaximumSpeed = maximumSpeed;
        }
    }
}