namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Contains the datas of location: speed, latitude, longitude, distance, up, down, type of speed.
    /// </summary>
    public class LocationData
    {
        /// <summary>
        /// The current speed in km/h.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// The current latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The current longitude.
        /// </summary>
        public double Longitude { get; set; }
        
        /// <summary>
        /// The distance from the previous location in meter.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The vertical up distance from the previous location in meter.
        /// </summary>
        public double Up { get; set; }

        /// <summary>
        /// The vertical down distance from the previous location in meter.
        /// </summary>
        public double Down { get; set; }

        /// <summary>
        /// The type of running at the current location.
        /// </summary>
        public RunningSpeed RunningSpeedType { get; set; }
        
        public LocationData(double longitude, double latitude, double speed = 0, double distance = 0, double up = 0, double down = 0, RunningSpeed runningSpeed = RunningSpeed.Unknown)
        {
            Speed = speed;
            Longitude = longitude;
            Latitude = latitude;
            Distance = distance;
            Up = up;
            Down = down;
            RunningSpeedType = runningSpeed;
        }
    }
}