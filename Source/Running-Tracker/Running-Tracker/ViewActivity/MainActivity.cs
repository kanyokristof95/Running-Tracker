﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content.PM;
using Running_Tracker.Model;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity, ILocationListener, IOnMapReadyCallback
    {
        public enum MainButtonStates
        {
            Calibrating,
            Start,
            Stop
        }

        public MainButtonStates MainButtonState { get; set; }

        private GoogleMap _map;
        private MapFragment _mapFragment;
        private LatLng lastUserPosition;

        private TextView txtTime;
   
        private Button mainButton;

        private LocationManager locationManager;
        private string locationProvider;

        private Dialog dialog;

        TextView speedTextView;
        TextView distanceTextView;
        double sumDistance;

        List<LatLng> lines;
        List<LatLng> circles;
        PolylineOptions rectOptions;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            //view-n lévő vezérlők megkeresése
            speedTextView = FindViewById<TextView>(Resource.Id.speed);
            distanceTextView = FindViewById<TextView>(Resource.Id.distance);
           

            //inicializáció
            MainButtonState = MainButtonStates.Calibrating;
            sumDistance = 0;
            lines = new List<LatLng>();
            circles = new List<LatLng>();
            rectOptions = new PolylineOptions().InvokeColor(Color.Red).InvokeZIndex(5);


            //toolbar
            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Running Tracker";

            mainButton = FindViewById<Button>(Resource.Id.mainButton);
            txtTime = FindViewById<TextView>(Resource.Id.txtTimer);
            mainButton.Click += MainButton_Clicked;

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

       

        private void Model_UserPosition(object sender, PositionArgs e)
        {
            _map.Clear();
            lastUserPosition = new LatLng(e.LocationData.Latitude, e.LocationData.Longitude);
            DrawCircle(lastUserPosition, 9, Color.Rgb(69, 140, 228), Color.White, 2);
            MoveCamera(lastUserPosition);
        }

        private void Model_Warning(object sender, WarningArgs e)
        {
            Vibrator vibrator = (Vibrator) GetSystemService(VibratorService);
            vibrator.Vibrate(500);

            switch (e.Warning)
            {
                case WarningType.Distance:
                    Toast.MakeText(this, "You reached the required distance.", ToastLength.Long).Show();
                    break;
                case WarningType.SpeedFast:
                    Toast.MakeText(this, "You are too fast.", ToastLength.Long).Show();
                    break;
                case WarningType.SpeedSlow:
                    Toast.MakeText(this, "You are too slow.", ToastLength.Long).Show();
                    break;
                case WarningType.Time:
                    Toast.MakeText(this, "You reached the required time.", ToastLength.Long).Show();
                    break;
            }
        }


        private void Model_CurrentTimeSpan(object sender, TimeSpanArgs e)
        {
            int hour = e.Time.Hours;
            int min = e.Time.Minutes;
            int sec = e.Time.Seconds;

            RunOnUiThread(() =>
            {
                txtTime.Text = String.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
            });
        }

        private void Model_NewPosition(object sender, PositionArgs e)
        {
            lastUserPosition = new LatLng(e.LocationData.Latitude, e.LocationData.Longitude);

            //speed refresh
            speedTextView.Text = Math.Round(e.LocationData.Speed, 2).ToString();

            //distance refresh
            sumDistance = sumDistance + e.LocationData.Distance;
            distanceTextView.Text = Math.Round(sumDistance, 2).ToString();

            // map refresh
            _map.Clear();
            if (e.LocationData.RunningSpeedType == Persistence.RunningSpeed.Stop)
            {
                circles.Add(lastUserPosition);
            }
            else
            {
                lines.Add(lastUserPosition);
                rectOptions.Add(lines[lines.Count - 1]);
            }

            // Medium speed's color
            rectOptions.InvokeColor(Color.Rgb(51, 127, 192));

            //draw
            _map.AddPolyline(rectOptions);

            foreach (var circle in circles)
            {
                DrawCircle(circle, 14, Color.Rgb(213, 52, 58), Color.Black, 1);
            }
            
            // User's positon
            DrawCircle(lastUserPosition, 9, Color.Rgb(69, 140, 228), Color.White, 2);
            MoveCamera(lastUserPosition);
        }

        private void Model_GPS_Ready(object sender, PositionArgs e)
        {
            _map.Clear();

            lastUserPosition = new LatLng(e.LocationData.Latitude, e.LocationData.Longitude);
            DrawCircle(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude), 9, Color.Rgb(69, 140, 228), Color.White, 2);
            MoveCamera(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));

            if (!model.IsRunning)
            {
                mainButton.Text = "Start";
                MainButtonState = MainButtonStates.Start;
            }
        }
        

        private void MainButton_Clicked(object sender, EventArgs e)
        {

           switch (MainButtonState)
            {
                case MainButtonStates.Calibrating:
                    Toast.MakeText(this, "GPS calibration in progress, please wait.", ToastLength.Long).Show();
                    break;
                case MainButtonStates.Start:
                    try
                    {
                        model.StartRunning();
                        model.UserPosition -= Model_UserPosition;
                        _map.Clear();
                        distanceTextView.Text = "0";
                        speedTextView.Text = "0";
                        MainButtonState = MainButtonStates.Stop;
                        mainButton.Text = "Stop";
                    } catch(gpsNotReadyException)
                    {
                        Toast.MakeText(this, "GPS is not calibrated.", ToastLength.Long).Show();
                    }
                    break;
                case MainButtonStates.Stop:
                    model.StopRunning();
                    MainButtonState = MainButtonStates.Start;
                    mainButton.Text = "Start";
                    Toast.MakeText(this, "Running is saved", ToastLength.Long).Show();
                    break;
            }

            if(lastUserPosition != null)
            {
                DrawCircle(lastUserPosition, 9, Color.Rgb(69, 140, 228), Color.White, 2);
            }
        }

        public void OnMapReady(GoogleMap map)
        {
            _map = map;
            
            MoveCamera(new LatLng(47.162494, 19.503304), 6);

            //eseménykezelő
            model.CurrentRunningDuration += Model_CurrentTimeSpan;
            model.Warning += Model_Warning;
            model.UserPosition += Model_UserPosition;
            model.GPS_Ready += Model_GPS_Ready;
            model.NewPosition += Model_NewPosition;
        }
        
        private void MoveCamera(LatLng location, float zoom = 16)
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom((float) zoom);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            _map.MoveCamera(cameraUpdate);
        }

        private void DrawCircle(LatLng circleCenter, int radius, Color fillColor, Color strokeColor, int strokeWidth)
        {
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(circleCenter);
            circleOptions.InvokeRadius(radius);
            circleOptions.InvokeFillColor(fillColor);
            circleOptions.InvokeStrokeColor(strokeColor);
            circleOptions.InvokeStrokeWidth(strokeWidth);
            circleOptions.InvokeZIndex(10);

            _map.AddCircle(circleOptions);
        }
        private void GpsSettings()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                GpsProviderDisabled();
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
                
                if (!model.IsRunning)
                {
                    MainButtonState = MainButtonStates.Calibrating;
                    mainButton.Text = "Calibrating ...";
                }
                model.Calibrate();
            }
        }

        private void GpsProviderDisabled()
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
                GpsSettings();
            });

            alertDialog.SetPositiveButton("Settings", delegate
            {
                var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                StartActivity(intent);
            });

            dialog = alertDialog.Create();
            dialog.Show();
        }

        private void GpsProviderEnabled()
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
            GpsProviderDisabled();
        }

        public void OnProviderEnabled(string provider)
        {
            GpsProviderEnabled();
        }

        
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras){}
        

        protected override void OnResume()
        {
            base.OnResume();
            GpsSettings();
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

