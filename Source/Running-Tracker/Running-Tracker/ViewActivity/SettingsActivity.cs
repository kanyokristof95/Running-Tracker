using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Running_Tracker.Persistence;
using System;
using System.Globalization;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "SettingsActivity", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : BaseActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            // Toolbar settings
            var mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Settings";

            // Load the precious values
            var personalData = Model.CurrentPersonalDatas;
            var warningValues = Model.CurrentWarningValues;

            // Search the items on the view
            var heightBox = FindViewById<EditText>(Resource.Id.height);
            var weightBox = FindViewById<EditText>(Resource.Id.weight);
            var distanceBox = FindViewById<EditText>(Resource.Id.distance);
            var timeBox = FindViewById<EditText>(Resource.Id.time);
            var minSpeedBox = FindViewById<EditText>(Resource.Id.minSpeed);
            var maxSpeedBox = FindViewById<EditText>(Resource.Id.maxSpeed);
            var sexMaleBox = FindViewById<RadioButton>(Resource.Id.sexMale);
            var sexFemaleBox = FindViewById<RadioButton>(Resource.Id.sexFemale);

            // Show the previously saved values
            if (personalData!= null & warningValues != null)
            {
                heightBox.Text = personalData.Height.ToString();
                weightBox.Text = personalData.Weight.ToString();
                distanceBox.Text = warningValues.Distance.ToString(CultureInfo.InvariantCulture);
                minSpeedBox.Text = warningValues.MinimumSpeed.ToString(CultureInfo.InvariantCulture);
                maxSpeedBox.Text = warningValues.MaximumSpeed.ToString(CultureInfo.InvariantCulture);
                timeBox.Text = warningValues.Time.TotalMinutes.ToString(CultureInfo.InvariantCulture);
                sexMaleBox.Checked = personalData.Sex.Equals(Gender.Male);
                sexFemaleBox.Checked = personalData.Sex.Equals(Gender.Female);
            }

            //Save the adjusted values 
            var saveButton = FindViewById<Button>(Resource.Id.saveButton);

            saveButton.Click += delegate
            {
                var sex = sexMaleBox.Checked ? Gender.Male : Gender.Female;

                personalData = new PersonalData
                {
                    Height = int.Parse(heightBox.Text),
                    Weight = int.Parse(weightBox.Text),
                    Sex = sex
                };

                warningValues = new WarningValues
                {
                    Distance = int.Parse(distanceBox.Text),
                    Time = new TimeSpan(0, int.Parse(timeBox.Text), 0),
                    MinimumSpeed = int.Parse(minSpeedBox.Text),
                    MaximumSpeed = int.Parse(maxSpeedBox.Text)

                };
                Model.SaveSettings(personalData, warningValues);

                Toast.MakeText(this, "Successful saving!", ToastLength.Long).Show();
            };
        }

        /// <summary>
        /// Load menu from the resources
        /// </summary>
        /// <param name="menu"></param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.undo_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Handle the click on menuitem
        /// </summary>
        /// <param name="item"></param>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Resource.Id.undo)
            {
                OnBackPressed();
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}