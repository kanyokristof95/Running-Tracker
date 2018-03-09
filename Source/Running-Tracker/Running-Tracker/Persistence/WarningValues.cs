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
        int Distance { get; set; }

        int Time { get; set; }

        int MinimumSpeed { get; set; }

        int MaximumSpeed { get; set; }
    }
}