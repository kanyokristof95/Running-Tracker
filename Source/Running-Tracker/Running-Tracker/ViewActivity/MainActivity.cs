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
using System.Globalization;
using System.Linq;
using Android.Graphics;
using Android.Content.PM;
using System.Timers;
using Running_Tracker.Model;
using Running_Tracker.Persistence;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity, ILocationListener, IOnMapReadyCallback
    {
        private enum MainButtonStates
        {
            Calibrating,
            Start,
            Stop
        }
        
        private RunningSpeed _lastRunningSpeedType;
        private Polyline _previousStateOfCurrentPolyline;
        private PolylineOptions _polylineOptions;
        private Circle _userPosition;

        private LatLng _lastUserPosition;

        // Map and provider
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private LocationManager _locationManager;
        private string _locationProvider;

        private CameraPosition.Builder _builder;

        // View elements
        private Dialog _dialog;
        private Button _mainButton;
        private MainButtonStates _mainButtonState;
        private TextView _txtTime;
        private TextView _speedTextView;
        private TextView _distanceTextView;
        private double _sumDistance;

        private UserPositionTimer _currentPositionTimer;
        private const int Fps = 30;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            // Toolbar settings
            var mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Running Tracker";

            // Find view
            _speedTextView = FindViewById<TextView>(Resource.Id.speed);
            _distanceTextView = FindViewById<TextView>(Resource.Id.distance);
            _mainButton = FindViewById<Button>(Resource.Id.mainButton);
            _txtTime = FindViewById<TextView>(Resource.Id.txtTimer);

            // Initialization
            _mainButtonState = MainButtonStates.Calibrating;
            _sumDistance = 0;
            _previousStateOfCurrentPolyline = null;
            _polylineOptions = null;
            _lastUserPosition = null;
            _userPosition = null;
            _lastRunningSpeedType = RunningSpeed.StartPoint;

            // Current position timer
            _currentPositionTimer = new UserPositionTimer(Fps, Model.GpsMinTime);
            _currentPositionTimer.Elapsed += CurrentPositionTimerOnElapsed;

            // Eventhandler
            _mainButton.Click += MainButton_Clicked;

            // Map fragment
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            if (_mapFragment == null)
            {
                var mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeZoomGesturesEnabled(true)
                    .InvokeScrollGesturesEnabled(false)
                    .InvokeCompassEnabled(true)
                    .InvokeZOrderOnTop(false);

                var fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);
        }
        
        /// <summary>
        /// Map is ready to use
        /// </summary>
        /// <param name="map">Map which is ready to use</param>
        public void OnMapReady(GoogleMap map)
        {
            _map = map;
            _builder = CameraPosition.InvokeBuilder();

            MoveCamera(new LatLng(47.162494, 19.503304), 6);

            // Event handlers
            Model.CurrentRunningDuration += Model_CurrentTimeSpan;
            Model.Warning += Model_Warning;
            Model.UserPosition += Model_UserPosition;
            Model.GpsReady += Model_GPS_Ready;
            Model.NewPosition += Model_NewPosition;
            Model.UserStopped += Model_UserStopped;
        }

        /// <summary>
        /// Gps is ready to use
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">New position</param>
        private void Model_GPS_Ready(object sender, PositionArgs e)
        {
            if (!Model.RunningEnd)
            {
                var position = new LatLng(e.LocationData.Latitude, e.LocationData.Longitude);
                _lastUserPosition = position;
                DrawUser(_lastUserPosition);
                MoveCamera(position, 16);
            }

        if (!Model.IsRunning)
            {
                _mainButton.Text = "Start";
                _mainButtonState = MainButtonStates.Start;
            }
        }

        /// <summary>
        /// Mainbutton is clicked
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void MainButton_Clicked(object sender, EventArgs e)
        {
            switch (_mainButtonState)
            {
                case MainButtonStates.Calibrating:
                    Toast.MakeText(this, "GPS calibration in progress, please wait.", ToastLength.Long).Show();
                    break;
                case MainButtonStates.Start:
                    try
                    {
                        _lastRunningSpeedType = RunningSpeed.StartPoint;
                        Model.StartRunning();
                        _userPosition?.Remove();
                        _userPosition = null;
                        _polylineOptions = null;
                        _previousStateOfCurrentPolyline = null;

                        _map.Clear();
                        DrawUser(_lastUserPosition);
                        _distanceTextView.Text = "0";
                        _speedTextView.Text = "0";
                        _mainButtonState = MainButtonStates.Stop;
                        _mainButton.Text = "Stop";
                    }
                    catch (GpsNotReadyException)
                    {
                        Toast.MakeText(this, "GPS is not calibrated.", ToastLength.Long).Show();
                    }
                    break;
                case MainButtonStates.Stop:
                    var saved = Model.StopRunning();
                    _mainButtonState = MainButtonStates.Start;
                    _mainButton.Text = "Start";
                    if (saved)
                        Toast.MakeText(this, "Running was saved", ToastLength.Long).Show();
                    else
                        Toast.MakeText(this, "Running wasn't saved", ToastLength.Long).Show();
                    break;
            }
        }

        /// <summary>
        /// Model gets the current position
        /// </summary>
        private void Model_UserPosition(object sender, PositionArgs e)
        {
            var previousUserPosition = _lastUserPosition;
            _currentPositionTimer.Start(previousUserPosition, new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));
        }

        /// <summary>
        /// Add new point to the polyline
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">User's new position</param>
        private void Model_NewPosition(object sender, PositionArgs e)
        {

            var position = new LatLng(e.LocationData.Latitude, e.LocationData.Longitude);

            // Refresh texts
            _speedTextView.Text = Math.Round(e.LocationData.Speed, 2).ToString(CultureInfo.InvariantCulture);
            _sumDistance = _sumDistance + e.LocationData.Distance;
            _distanceTextView.Text = Math.Round(_sumDistance, 2).ToString(CultureInfo.InvariantCulture);

            // Add new line
            if (e.LocationData.RunningSpeedType != _lastRunningSpeedType)
            {
                LatLng lastPosition = null;

                if (_previousStateOfCurrentPolyline != null)
                {
                    lastPosition = _polylineOptions.Points.Last();
                }

                _previousStateOfCurrentPolyline = null;
                _lastRunningSpeedType = e.LocationData.RunningSpeedType;

                _polylineOptions = new PolylineOptions();

                switch (e.LocationData.RunningSpeedType)
                {
                    case RunningSpeed.Slow:
                        _polylineOptions.InvokeColor(Color.Rgb(238, 163, 54));
                        break;
                    case RunningSpeed.Normal:
                        _polylineOptions.InvokeColor(Color.Rgb(51, 127, 192));
                        break;
                    case RunningSpeed.Fast:
                        _polylineOptions.InvokeColor(Color.Rgb(33, 175, 95));
                        break;
                    case RunningSpeed.StartPoint:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _polylineOptions.InvokeZIndex(5);

                if (lastPosition != null)
                    _polylineOptions.Add(lastPosition);
            }
            _polylineOptions.Add(position);

            // Drawing the line
            var timer = new Timer();
            timer.Interval = Model.GpsMinTime / 2;
            timer.Elapsed += (o, args) =>
            {
                RunOnUiThread(() =>
                {
                    var currentPolyline = _map.AddPolyline(_polylineOptions);
                    _previousStateOfCurrentPolyline?.Remove();
                    _previousStateOfCurrentPolyline = currentPolyline;
                });
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// Refresh the running's time
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Current running's time</param>
        private void Model_CurrentTimeSpan(object sender, TimeSpanArgs e)
        {
            var hour = e.Time.Hours;
            var min = e.Time.Minutes;
            var sec = e.Time.Seconds;

            RunOnUiThread(() =>
            {
                _txtTime.Text = $"{hour:00}:{min:00}:{sec:00}";
            });
        }

        /// <summary>
        /// When the user stops.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">User's position</param>
        private void Model_UserStopped(object sender, PositionArgs e)
        {
            var circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(new LatLng(e.LocationData.Latitude, e.LocationData.Longitude));
            circleOptions.InvokeRadius(14);
            circleOptions.InvokeFillColor(Color.Rgb(213, 52, 58));
            circleOptions.InvokeStrokeColor(Color.Black);
            circleOptions.InvokeStrokeWidth(1);
            circleOptions.InvokeZIndex(10);

            RunOnUiThread(() =>
            {
                _map.AddCircle(circleOptions);
                _speedTextView.Text = "0";
            });
        }
        
        /// <summary>
        /// Warn the user if he/she is too slow/fast or reached the required distance/time
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Warning type</param>
        private void Model_Warning(object sender, WarningArgs e)
        {
            var vibrator = (Vibrator) GetSystemService(VibratorService);
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

        /// <summary>
        /// Draw the animated user position.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="positionArgs">User's position at the current frame</param>
        private void CurrentPositionTimerOnElapsed(object sender, PositionArgs positionArgs)
        {
            _lastUserPosition = new LatLng(positionArgs.LocationData.Latitude, positionArgs.LocationData.Longitude);
            DrawUser(_lastUserPosition);
            MoveCamera(_lastUserPosition, null);
        }
        
        /// <summary>
        /// Delete the previous user circle and draw the new one
        /// </summary>
        /// <param name="position">User's new position</param>
        private void DrawUser(LatLng position)
        {

            var circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(position);
            circleOptions.InvokeRadius(9);
            circleOptions.InvokeFillColor(Color.Rgb(69, 140, 228));
            circleOptions.InvokeStrokeColor(Color.White);
            circleOptions.InvokeStrokeWidth(2);
            circleOptions.InvokeZIndex(11);
            
            RunOnUiThread(() =>
            {
                var newUserPosition = _map.AddCircle(circleOptions);
                _userPosition?.Remove();
                _userPosition = newUserPosition;
            });
        }


        /// <summary>
        /// Move the camera into the user's position
        /// </summary>
        /// <param name="location">User's new position</param>
        /// <param name="zoom">Zoom value (if it's null, won't be any zoom)</param>
        private void MoveCamera(LatLng location, float? zoom)
        {
            RunOnUiThread(() =>
            {
                _builder.Target(location);

                if (zoom != null)
                    _builder.Zoom((float) zoom);
                else
                    _builder.Zoom(_map.CameraPosition.Zoom);

                _builder.Bearing(_map.CameraPosition.Bearing);


                var cameraPosition = _builder.Build();
                var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                
                _map.MoveCamera(cameraUpdate);
            });
        }

        /// <summary>
        /// Configure the GPS
        /// </summary>
        private void GpsSettings()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);

            if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
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

                _locationProvider = _locationManager.GetBestProvider(locationCriteria, true);

                if (_locationProvider == null)
                {
                    _locationProvider = string.Empty;
                }
                
                _locationManager.RequestLocationUpdates(_locationProvider, Model.GpsMinTime, Model.GpsMinDistance, this);
                
                if (!Model.IsRunning)
                {
                    _mainButtonState = MainButtonStates.Calibrating;
                    _mainButton.Text = "Calibrating ...";
                }
                Model.Calibrate();
            }
        }

        /// <summary>
        /// GPS provider is disabled
        /// </summary>
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

            _dialog = alertDialog.Create();
            _dialog.Show();
        }

        /// <summary>
        /// GPS provider is enabled
        /// </summary>
        private void GpsProviderEnabled()
        {
            if (_dialog != null)
            {
                _dialog.Dismiss();
                _dialog = null;
            }

            Toast.MakeText(this, "GPS provider is enabled", ToastLength.Long).Show();
        }

        /// <summary>
        /// GPS provider sent a new location
        /// </summary>
        /// <param name="location"></param>
        public void OnLocationChanged(Location location)
        {
            Model.ChangeLocation(location);
        }

        /// <summary>
        /// GPS provider is disabled
        /// </summary>
        /// <param name="provider">GPS provider</param>
        public void OnProviderDisabled(string provider)
        {
            GpsProviderDisabled();
        }

        /// <summary>
        /// GPS provider is enabled
        /// </summary>
        /// <param name="provider">GPS provider</param>
        public void OnProviderEnabled(string provider)
        {
            GpsProviderEnabled();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) {}
        
        /// <summary>
        /// Resume the activity
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            GpsSettings();
        }

        /// <summary>
        /// Pause the activity
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        /// <summary>
        /// Load the menu from resource
        /// </summary>
        /// <param name="menu"></param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Handle the click on menuitem. 
        /// </summary>
        /// <param name="item"></param>
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

