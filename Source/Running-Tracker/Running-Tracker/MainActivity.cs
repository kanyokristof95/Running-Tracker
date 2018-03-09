using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;

namespace Running_Tracker
{
    [Activity(Label = "Running_Tracker", MainLauncher = true, Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Running Tracker";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent = null;
            switch (item.ItemId)
            {
                case Resource.Id.history:
                    intent = new Intent(this, typeof(HistoryActivity));
                    break;
                case Resource.Id.settings:
                    intent = new Intent(this, typeof(SettingsActivity));
                    break;
                default:
                    break;
            }

            if (intent != null)
            {
                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}

