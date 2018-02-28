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
using static Android.Widget.AdapterView;


namespace View
{
    [Activity(Label = "second")]
    public class second : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {

          base.OnCreate(bundle);

          SetContentView(Resource.Layout.Second);

          Button button = FindViewById<Button>(Resource.Id.First);
          button.Click += delegate {
              StartActivity(typeof(BasicListView));
          };

          Button button2 = FindViewById<Button>(Resource.Id.Grid);
            button2.Click += delegate {
                StartActivity(typeof(ComplexLayout));
           };
        }
    }
}