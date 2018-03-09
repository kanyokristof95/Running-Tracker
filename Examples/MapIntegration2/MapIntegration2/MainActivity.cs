using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Timers;

namespace MapIntegration2
{
    [Activity(Label = "MapIntegration2", MainLauncher = true)]
    public class MainActivity : Activity, IOnMapReadyCallback
    {
        GoogleMap _map;
        MapFragment _mapFragment;

        Timer timer;
        int i = 0;

        List<LatLng> lines;
        PolylineOptions rectOptions;

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

            /*PolylineOptions rectOptions = new PolylineOptions().InvokeColor(Color.Red);
            
            rectOptions.Add(new LatLng(37.785559, -122.396728));
            rectOptions.Add(new LatLng(37.780624, -122.390541));
            rectOptions.Add(new LatLng(37.777113, -122.394983));
            rectOptions.Add(new LatLng(37.776831, -122.394627));
            rectOptions.Add(new LatLng(37.776831, -122.404627));

            _map.AddPolyline(rectOptions);*/
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

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

            rectOptions = new PolylineOptions().InvokeColor(Color.Red);

            lines = new List<LatLng>
            {
                new LatLng(37.785559, -122.396728), // 0
                new LatLng(37.780624, -122.390541), // 1
                new LatLng(37.777113, -122.394983), // 2
                new LatLng(37.776831, -122.394627), // 3
                new LatLng(37.776831, -122.404627) // 4
            };

            _mapFragment.GetMapAsync(this);

            timer = new Timer
            {
                Interval = 5000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_map != null && i < lines.Count)
            {
                RunOnUiThread(() =>
                {
                    // Ebben az esetben a szakaszokat egymástól függetlenül rajzolja le, amelynek hatására kevésbé lesz szép
                    /*rectOptions = new PolylineOptions().InvokeColor(Color.Red);
                    rectOptions.Add(lines[i]);
                    rectOptions.Add(lines[i + 1]);
                    i++;
                    _map.AddPolyline(rectOptions); // Az i. és az i+1. közötti szakasz kirajzolása*/

                    // Ebben az esetben a pontokat összekötő egyeneseket egyben kezeli, amely hatására szebb lesz a kirajzolása
                    _map.Clear();
                    rectOptions.Add(lines[i]);
                    i++;
                    if (i == 1)
                    {
                        rectOptions.Add(lines[i]);
                        i++;
                    }
                    _map.AddPolyline(rectOptions);
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    i = 0;
                    rectOptions = new PolylineOptions().InvokeColor(Color.Red);
                    _map.Clear(); // A térképre rajzolás megtisztítása
                });

            }
        }
    }
}

