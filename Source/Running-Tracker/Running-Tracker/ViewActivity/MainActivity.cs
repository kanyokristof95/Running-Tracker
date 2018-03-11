using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Timers;
using System;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "Running_Tracker", MainLauncher = true, Theme = "@style/MyTheme")]
    public class MainActivity : BaseActivity, ILocationListener, IOnMapReadyCallback
    {
        GoogleMap _map;
        MapFragment _mapFragment;

        Timer secCounter;
        int hour, min, sec;
        TextView txtTime;
        
        private LocationManager locationManager;
        private string locationProvider;

        private Dialog dialog;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            //toolbar
            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Running Tracker";

            //timer
            hour = 0;
            min = 0;
            sec = 0;
            Button mainButton = FindViewById<Button>(Resource.Id.mainButton);
            txtTime = FindViewById<TextView>(Resource.Id.txtTimer);
            mainButton.Click += StartButton_Click;

            //map integráció
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);

           
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            //TODO AZ ELEJÉN CALCULATING FELIRAT, NEM KATTINTHATÓ, AZUTÁN START, VÉGÉN STOP
            secCounter = new Timer();
            secCounter.Interval = 1000;
            secCounter.Elapsed += SecCounterTimerElapsed;
            secCounter.Start();
        }

        private void SecCounterTimerElapsed(object sender, ElapsedEventArgs e)
        {
            sec++;
            if(sec == 60)
            {
                min++;
                sec = 0;
            }
            if(min == 60)
            {
                hour++;
                min = 0;
            }
            RunOnUiThread(() =>
           {
           txtTime.Text = String.Format("{0:00}:{1:00}:{2:00}", hour, min, sec); 
           });
        }

        public void OnMapReady(GoogleMap map)
        {
            _map = map;

            LatLng location = new LatLng(37.785559, -122.396728);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(14);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            _map.MoveCamera(cameraUpdate);
        }

        private void gpsSettings()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                gpsProviderDisabled();
            }
            else
            {
                Criteria locationCriteria = new Criteria
                {
                    Accuracy = Accuracy.Fine,
                    PowerRequirement = Power.High
                };

                locationProvider = locationManager.GetBestProvider(locationCriteria, true);

                if (locationProvider == null)
                {
                    locationProvider = string.Empty;
                }
                
                locationManager.RequestLocationUpdates(locationProvider, model.GPSMinTime, model.GPSMinDistance, this);
                model.Calibrate();
            }
        }

        private void gpsProviderDisabled()
        {
            var alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("GPS provider");
            alertDialog.SetMessage("Please turn on the GPS provider!");
            alertDialog.SetNegativeButton("Exit", delegate
            {
                Finish();
            });

            alertDialog.SetNeutralButton("Retry", delegate
            {
                // If the user start the app without gps, then the OnProviderEnabled function won't be called
                // In this case the user has to click into the retry button
                gpsSettings();
            });

            alertDialog.SetPositiveButton("Settings", delegate
            {
                var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                StartActivity(intent);
            });

            dialog = alertDialog.Create();
            dialog.Show();
        }

        private void gpsProviderEnabled()
        {
            if (dialog != null)
            {
                dialog.Dismiss();
                dialog = null;
            }

            Toast.MakeText(this, "GPS provider is enabled", ToastLength.Long).Show();
        }
        
        public void OnLocationChanged(Location location)
        {
            model.ChangeLocation(location);
        }

        public void OnProviderDisabled(string provider)
        {
            gpsProviderDisabled();
        }

        public void OnProviderEnabled(string provider)
        {
            gpsProviderEnabled();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            switch(status)
            {
                case Availability.Available:
                    Toast.MakeText(this, "Status changed: GPS provider is available", ToastLength.Long).Show();
                    break;
                case Availability.TemporarilyUnavailable:
                    Toast.MakeText(this, "Status changed: GPS provider is temporarily unavailable", ToastLength.Long).Show();
                    break;
                case Availability.OutOfService:
                    Toast.MakeText(this, "Status changed: GPS provider is out of service", ToastLength.Long).Show();
                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            gpsSettings();
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
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

