using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Running_Tracker.Persistence;

namespace Running_Tracker.Model
{
    public class PositionArgs : EventArgs
    {
        private LocationData locationData;

        public PositionArgs(LocationData locationData)
        {
            this.locationData = locationData;
        }

        public LocationData LocationData
        {
            get { return locationData; }
        }
    }
}