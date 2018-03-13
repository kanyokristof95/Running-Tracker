using Android.Locations;

namespace Running_Tracker.Persistence
{
    public class LocationData
    {
        /// <summary>
        /// km/h
        /// </summary>
        public double Speed { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }


        /// <summary>
        /// In meter
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// In meter
        /// </summary>
        public double Up { get; private set; }

        /// <summary>
        /// In meter
        /// </summary>
        public double Down { get; private set; }

        public RunningSpeed RunningSpeedType { get; private set; }
        
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