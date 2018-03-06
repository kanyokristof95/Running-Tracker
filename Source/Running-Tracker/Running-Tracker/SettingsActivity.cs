
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;

namespace Running_Tracker
{
    [Activity(Label = "SettingsActivity", Theme = "@style/MyTheme")]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Settings";


            //Examples
            NumberPicker heightNumberPicker = FindViewById<NumberPicker>(Resource.Id.heightNumberPicker);
            heightNumberPicker.MinValue = 0;
            heightNumberPicker.MaxValue = 10;
            heightNumberPicker.SetDisplayedValues(new String[] { "0.0 meter", "0.1 meter", "0.2 meter", "0.3 meter", "0.4 meter", "0.5 meter", "0.6 meter", "0.7 meter", "0.8 meter", "0.9 meter", "1.0 meter" });

            NumberPicker weightNumberPicker = FindViewById<NumberPicker>(Resource.Id.weightNumberPicker);
            weightNumberPicker.MinValue = 0;
            weightNumberPicker.MaxValue = 5;
            weightNumberPicker.SetDisplayedValues(new String[] { "0.0 kg", "0.1 kg", "0.2 kg", "0.3 kg", "0.4 kg", "0.5 kg"});

            NumberPicker distanceNumberPicker = FindViewById<NumberPicker>(Resource.Id.distanceNumberPicker);
            distanceNumberPicker.MinValue = 0;
            distanceNumberPicker.MaxValue = 5;
            distanceNumberPicker.SetDisplayedValues(new String[] { "0.0 meter", "0.1 meter", "0.2 meter", "0.3 meter", "0.4 meter", "0.5 meter" });

            NumberPicker timeNumberPicker = FindViewById<NumberPicker>(Resource.Id.timeNumberPicker);
            timeNumberPicker.MinValue = 0;
            timeNumberPicker.MaxValue = 5;
            timeNumberPicker.SetDisplayedValues(new String[] { "0 min", "1 min", "2 min", "3 min", "4 min", "5 min" });

            NumberPicker minSpeedNumberPicker = FindViewById<NumberPicker>(Resource.Id.minSpeedNumberPicker);
            minSpeedNumberPicker.MinValue = 0;
            minSpeedNumberPicker.MaxValue = 5;
            minSpeedNumberPicker.SetDisplayedValues(new String[] { "<0.0 km/h", "<0.1 km/h", "<0.2 km/h", "<0.3 km/h", "<0.4 km/h", "<0.5 km/h" });

            NumberPicker maxSpeedNumberPicker = FindViewById<NumberPicker>(Resource.Id.maxSpeedNumberPicker);
            maxSpeedNumberPicker.MinValue = 0;
            maxSpeedNumberPicker.MaxValue = 5;
            maxSpeedNumberPicker.SetDisplayedValues(new String[] { ">0.0 km/h", ">0.1 km/h", ">0.2 km/h", ">0.3 km/h", ">0.4 km/h", ">0.5 km/h" });

            Button saveButton = FindViewById<Button>(Resource.Id.saveButton);
            saveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            //TODO BEÁLLÍTOTT ADATOK ELMENTÉSE
            Console.WriteLine("Save button clicked");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.undo_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent = null;
         
            if(item.ItemId == Resource.Id.undo)
            {
                intent = new Intent(this, typeof(MainActivity));
            }

            if (intent != null)
            {
                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}