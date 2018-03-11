using Android.Locations;

namespace Running_Tracker.Persistence
{
    public class LocationData : Location
    {
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
        
        public LocationData(Location location, double distance = 0, double up = 0, double down = 0) : base(location)
        {
            Distance = distance;
            Up = up;
            Down = down;
        }
    }
}