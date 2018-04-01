
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Running_Tracker.Persistence;
using System;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "SettingsActivity", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : BaseActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Settings";

            PersonalData personalData = model.CurrentPersonalDatas;
            WarningValues warningValues = model.CurrentWarningValues;

            EditText heightBox = FindViewById<EditText>(Resource.Id.height);
            EditText weightBox = FindViewById<EditText>(Resource.Id.weight);
            EditText distanceBox = FindViewById<EditText>(Resource.Id.distance);
            EditText timeBox = FindViewById<EditText>(Resource.Id.time);
            EditText minSpeedBox = FindViewById<EditText>(Resource.Id.minSpeed);
            EditText maxSpeedBox = FindViewById<EditText>(Resource.Id.maxSpeed);
            RadioButton sexMaleBox = FindViewById<RadioButton>(Resource.Id.sexMale);
            RadioButton sexFemaleBox = FindViewById<RadioButton>(Resource.Id.sexFemale);


            if (personalData!= null & warningValues != null)
            {
                heightBox.Text = personalData.Height.ToString();
                weightBox.Text = personalData.Weight.ToString();
                distanceBox.Text = warningValues.Distance.ToString();
                minSpeedBox.Text = warningValues.MinimumSpeed.ToString();
                maxSpeedBox.Text = warningValues.MaximumSpeed.ToString();
                timeBox.Text = warningValues.Time.TotalMinutes.ToString();
                sexMaleBox.Checked = personalData.Sex.Equals(Gender.Male);
                sexFemaleBox.Checked = personalData.Sex.Equals(Gender.Female);


            }


            Button saveButton = FindViewById<Button>(Resource.Id.saveButton);

            saveButton.Click += delegate
            {
                Gender sex = sexMaleBox.Checked ? Gender.Male : Gender.Female;

                personalData = new PersonalData
                {
                    Height = Int32.Parse(heightBox.Text),
                    Weight = Int32.Parse(heightBox.Text),
                    Sex = sex
                };

                warningValues = new WarningValues
                {
                    Distance = Int32.Parse(distanceBox.Text),
                    Time = new TimeSpan(0, Int32.Parse(timeBox.Text), 0),
                    MinimumSpeed = Int32.Parse(minSpeedBox.Text),
                    MaximumSpeed = Int32.Parse(maxSpeedBox.Text)

                };
                model.SaveSettings(personalData, warningValues);

                Toast.MakeText(this, "Successful saving!", ToastLength.Long).Show();

            };


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.undo_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Resource.Id.undo)
            {
                base.OnBackPressed();
            }

            return base.OnOptionsItemSelected(item);
        }
    }

}