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
    public class PersonalDatas
    {
        public Gender Sex { get; set; }

        public int Height { get; set; }

        public int Weight { get; set; }
    }
}