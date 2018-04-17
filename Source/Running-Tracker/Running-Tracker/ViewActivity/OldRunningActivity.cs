using System;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Content.PM;
using Running_Tracker.Persistence;
using Newtonsoft.Json;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "OldRunningActivity", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class OldRunningActivity : BaseActivity , IOnMapReadyCallback
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private RunningData _running;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.OldRunning);

            // Toolbar
            var mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Run details";
            
            // Map
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            if (_mapFragment == null)
            {
                var mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                var fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.old_running_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnMapReady(GoogleMap map)
        {
            _map = map;
            _running = JsonConvert.DeserializeObject<RunningData>(Intent.GetStringExtra("Running"));

            var b = new LatLngBounds.Builder()
                .Include(new LatLng(_running.MinLatitude, _running.MinLongitude))
                .Include(new LatLng(_running.MaxLatitude, _running.MaxLongitude));
            _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(b.Build(), 120));
            
            var lastPointSpeedType = RunningSpeed.StartPoint;

            PolylineOptions polylineOptions = null;
            LatLng lastPosition = null;

            foreach(var point in _running.Locations)
            {
                if (point.RunningSpeedType != lastPointSpeedType)
                {
                    lastPointSpeedType = point.RunningSpeedType;
                    if (polylineOptions != null)
                    {
                        _map.AddPolyline(polylineOptions);

                        lastPosition = polylineOptions.Points.Last();
                        polylineOptions = new PolylineOptions();
                    }
                    else
                    {
                        polylineOptions = new PolylineOptions();
                    }
                    
                    switch (point.RunningSpeedType)
                    {
                        case RunningSpeed.Slow:
                            polylineOptions.InvokeColor(Color.Rgb(238, 163, 54));
                            break;
                        case RunningSpeed.Normal:
                            polylineOptions.InvokeColor(Color.Rgb(51, 127, 192));
                            break;
                        case RunningSpeed.Fast:
                            polylineOptions.InvokeColor(Color.Rgb(33, 175, 95));
                            break;
                        case RunningSpeed.StartPoint:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (lastPosition != null)
                        polylineOptions.Add(lastPosition);
                }

                polylineOptions?.Add(new LatLng(point.Latitude, point.Longitude));
            }
            
            _map.AddPolyline(polylineOptions);

            foreach (var circle in _running.Stops)
            {
                var circleOptions = new CircleOptions();
                circleOptions.InvokeCenter(new LatLng(circle.Latitude, circle.Longitude));
                circleOptions.InvokeRadius(14);
                circleOptions.InvokeFillColor(Color.Rgb(213, 52, 58));
                circleOptions.InvokeStrokeColor(Color.Black);
                circleOptions.InvokeStrokeWidth(1);
                circleOptions.InvokeZIndex(10);

                _map.AddCircle(circleOptions);
            }

            var hour = _running.Duration.Hours;
            var min = _running.Duration.Minutes;
            var sec = _running.Duration.Seconds;

            FindViewById<TextView>(Resource.Id.txtTimer).Text = $"{hour:00}:{min:00}:{sec:00}";
            FindViewById<TextView>(Resource.Id.distance).Text = Math.Round(_running.Distance, 2).ToString(CultureInfo.InvariantCulture);
            FindViewById<TextView>(Resource.Id.avgSpeed).Text = Math.Round(_running.AverageSpeed, 2).ToString(CultureInfo.InvariantCulture);
            FindViewById<TextView>(Resource.Id.up).Text = Math.Round(_running.Up, 2).ToString(CultureInfo.InvariantCulture);
            FindViewById<TextView>(Resource.Id.down).Text = Math.Round(_running.Down, 2).ToString(CultureInfo.InvariantCulture);
            FindViewById<TextView>(Resource.Id.calorie).Text = Math.Round(_running.Calorie, 2).ToString(CultureInfo.InvariantCulture);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            if(item.ItemId == Resource.Id.undo)
            {
                OnBackPressed();
            } else if (item.ItemId == Resource.Id.delete)
            {
                var alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDialog.SetTitle("Delete running");
                alertDialog.SetMessage("Are you sure you want delete this run?");
                alertDialog.SetNegativeButton("No", delegate
                {
                    
                });

                alertDialog.SetPositiveButton("Yes", delegate
                {
                    model.DeleteRunning(_running);
                    OnBackPressed();
                });
                alertDialog.Show();
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}