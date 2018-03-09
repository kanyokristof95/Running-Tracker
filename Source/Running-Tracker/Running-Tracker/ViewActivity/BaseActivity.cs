using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Running_Tracker.Model;
using Running_Tracker.Persistence;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected RunningTrackerModel model;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            model = new RunningTrackerModel(new RunningTrackerDataAccess());   
        }
    }
}