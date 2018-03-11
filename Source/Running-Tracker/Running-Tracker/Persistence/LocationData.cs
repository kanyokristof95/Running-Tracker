using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
        
        public LocationData(Location location, double distance, double up, double down) : base(location)
        {
            Distance = distance;
            Up = up;
            Down = down;
        }
    }
}