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