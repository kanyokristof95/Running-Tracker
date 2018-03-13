using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Timers;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content.PM;

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

        public MainButtonStates MainButtonState{ get; set; }
        

        GoogleMap _map;
        MapFragment _mapFragment;

        int hour, min, sec;
        TextView txtTime;

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
            

            //gps eseménykezelő
             model.GPS_Ready += Model_GPS_Ready;
             model.NewPosition += Model_NewPosition;
            model.CurrentTimeSpan += Model_CurrentTimeSpan;


            //inicializáció
            MainButtonState = MainButtonStates.Calibrating;
            sumDistance = 0;
            lines = new List<LatLng>();
            circles = new List<LatLng>();
            rectOptions = new PolylineOptions().InvokeColor(Color.Red);


            //toolbar
            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Running Tracker";

            //timer
            hour = 0;
            min = 0;
            sec = 0;
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

        private void Model_CurrentTimeSpan(object sender, Model.TimeSpanArgs e)
        {
            hour = e.Time.Hours;
            min = e.Time.Minutes;
            sec = e.Time.Seconds;

            RunOnUiThread(() =>
            {
                txtTime.Text = String.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
            });
        }

        private void Model_NewPosition(object sender, Model.PositionArgs e)
        {
            Toast.MakeText(this, "Model_new_position", ToastLength.Long).Show();
            //speed refresh
            speedTextView.Text = e.LocationData.Speed.ToString();

            //distance refresh
            sumDistance = sumDistance + e.LocationData.Distance;
            distanceTextView.Text = sumDistance.ToString();

            // map refresh
            _map.Clear();
            if (e.LocationData.RunningSpeedType == Persistence.RunningSpeed.Stop)
            {
                circles.Add(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));
            }
            else
            {
                lines.Add(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));
                rectOptions.Add(lines[lines.Count - 1]);
            }
            //draw
            _map.AddPolyline(rectOptions);

            foreach (var circle in circles)
            {
                drawCircle(circle);
            }

            //user current position
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));
            circleOptions.InvokeRadius(9);
            circleOptions.InvokeStrokeColor(Color.White);
            circleOptions.InvokeFillColor(Color.Rgb(69, 140, 228));
            circleOptions.InvokeStrokeWidth(2);

            _map.AddCircle(circleOptions);

        }

        private void Model_GPS_Ready(object sender, Model.PositionArgs e)
        {
            LatLng location = new LatLng(e.LocationData.Latitude,e.LocationData.Longitude);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(16);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            _map.MoveCamera(cameraUpdate);

           if(MainButtonState == MainButtonStates.Calibrating){ 
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
                    distanceTextView.Text = "0";
                    MainButtonState = MainButtonStates.Stop;
                    mainButton.Text = "Stop";
                    model.StartRunning();
                    break;
                case MainButtonStates.Stop:
                    model.StopRunning();
                    MainButtonState = MainButtonStates.Start;
                    mainButton.Text = "Start";
                    //TODO inicializálás alaphelyzetre
                    break;
            }     
        }

        public void OnMapReady(GoogleMap map)
        {
            _map = map;

            LatLng location = new LatLng(47.162494, 19.503304);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(6);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            _map.MoveCamera(cameraUpdate);
            
        }
        private void drawCircle(LatLng circleCenter)
        {
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(circleCenter);
            circleOptions.InvokeRadius(35);
            //circleOptions.InvokeFillColor(Color.Rgb(213, 52, 58));
            circleOptions.InvokeStrokeColor(Color.Black);
            circleOptions.InvokeFillColor(0x30ff0000);
            circleOptions.InvokeStrokeWidth(2);

            _map.AddCircle(circleOptions);
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

        
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras){}
        

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

