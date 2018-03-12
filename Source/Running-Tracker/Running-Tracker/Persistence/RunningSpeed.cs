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
    public enum RunningSpeed
    {
        Unknown,
        /// <summary>
        /// The first point of the running
        /// </summary>
        StartPoint,
        /// <summary>
        /// When the runner stops
        /// </summary>
        Stop,
        /// <summary>
        /// Slow running
        /// </summary>
        Slow,
        /// <summary>
        /// Normal running 
        /// </summary>
        Normal,
        /// <summary>
        /// Fast running
        /// </summary>
        Fast
    }
}