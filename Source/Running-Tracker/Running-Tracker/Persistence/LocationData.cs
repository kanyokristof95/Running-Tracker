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
        public MovementType Movement { get; set; }

        public LocationData(Location location) : base(location)
        {
            Movement = MovementType.Unknown;
        }
    }
}