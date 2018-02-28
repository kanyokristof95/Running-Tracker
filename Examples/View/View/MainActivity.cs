using Android.App;
using Android.Widget;
using Android.OS;

namespace View
{
    [Activity(Label = "View", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
    
         base.OnCreate(savedInstanceState);
         SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.sec);
            button.Click += delegate {
                StartActivity(typeof(second));
            };














        }
    }
}

