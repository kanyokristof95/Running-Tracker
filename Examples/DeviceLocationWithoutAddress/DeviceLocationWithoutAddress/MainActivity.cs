using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using Android.Util;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Text;
using Android.Runtime;

namespace DeviceLocationWithoutAddress
{
    [Activity(Label = "DeviceLocationWithoutAddress", MainLauncher = true)]
    public class MainActivity : Activity, ILocationListener
    {
        TextView _LongitudeText;
        TextView _LatitudeText;

        Location _currentLocation;
        LocationManager _locationManager;

        string _locationProvider;  

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _LongitudeText.Text = "Loading ...";
                _LatitudeText.Text = "Loading ...";
            }
            else
            {
                _LongitudeText.Text = _currentLocation.Longitude.ToString();
                _LatitudeText.Text = _currentLocation.Latitude.ToString();
            }
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            _LongitudeText = FindViewById<TextView>(Resource.Id.LongitudeText);
            _LatitudeText = FindViewById<TextView>(Resource.Id.LatitudeText);
            
            InitializeLocationManager();
        }

        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First(); // GPS
            }
            else
            {
                _locationProvider = string.Empty;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this); // Beállítja, hogy a helyzet megváltoztatása esetén küldjön jelet
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }
    }
}

