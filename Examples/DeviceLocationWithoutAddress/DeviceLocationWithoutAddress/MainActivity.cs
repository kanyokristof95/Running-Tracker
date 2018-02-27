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
using Android.Views;
using Android.Content.PM;

namespace DeviceLocationWithoutAddress
{
    [Activity(Label = "DeviceLocationWithoutAddress", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity, ILocationListener
    {
        double distance = 0; // In meter

        TextView _SpeedText;
        TextView _DistanceText;
        TextView _LongitudeText;
        TextView _LatitudeText;
        TextView _AltitudeText;
        TextView _ProviderText;
        TextView _AccuracyText;

        Location _previousLocation = null;
        Location _currentLocation = null;

        LocationManager _locationManager;
        string _locationProvider;  

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;

            _AccuracyText.Text = Math.Round(_currentLocation.Accuracy, 2).ToString();

            if (_currentLocation.Accuracy > 5)
                return;

            if (_currentLocation == null)
            {
                _SpeedText.Text = "0 km/h";
                _LongitudeText.Text = "Waiting for GPS ...";
                _LatitudeText.Text = "Waiting for GPS ...";
                _AltitudeText.Text = "Waiting for GPS ...";
                _ProviderText.Text = "Waiting for GPS ...";
                _AccuracyText.Text = "Waiting for GPS ...";
            }
            else
            {
                _SpeedText.Text = Math.Round(_currentLocation.Speed * 3.6, 2).ToString() + " km/h";
                if (_previousLocation != null)
                {
                    distance += _currentLocation.DistanceTo(_previousLocation);
                    distance = Math.Round(distance, 2);
                    _DistanceText.Text = distance.ToString() + " m";
                }
                _LongitudeText.Text = _currentLocation.Longitude.ToString();
                _LatitudeText.Text = _currentLocation.Latitude.ToString();
                _AltitudeText.Text = _currentLocation.Altitude.ToString();
                _ProviderText.Text = _currentLocation.Provider;
            }

            _previousLocation = location;
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

            // Letiltja az alvó módot
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);


            _SpeedText = FindViewById<TextView>(Resource.Id.SpeedText);
            _DistanceText = FindViewById<TextView>(Resource.Id.DistanceText);
            _LongitudeText = FindViewById<TextView>(Resource.Id.LongitudeText);
            _LatitudeText = FindViewById<TextView>(Resource.Id.LatitudeText);
            _AltitudeText = FindViewById<TextView>(Resource.Id.AltitudeText);
            _ProviderText = FindViewById<TextView>(Resource.Id.ProviderText);
            _AccuracyText = FindViewById<TextView>(Resource.Id.AccuracyText);

            InitializeLocationManager();
        }

        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager) GetSystemService(LocationService);
            
            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Fine;
            locationCriteria.PowerRequirement = Power.High;

            _locationProvider = _locationManager.GetBestProvider(locationCriteria, true); // A legjobb elérhető provider-t adja

            // Nincs elérhető provider
            if (_locationProvider == null)
            {
                _locationProvider = string.Empty;
            }


            /*Criteria criteriaForLocationService = new Criteria
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
            }*/
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Csak akkor küld jelet, ha eltelt 1 mp és legalább 0 métert megtett
            _locationManager.RequestLocationUpdates(_locationProvider, 1000, 0, this);

            // csak 1 szignál küldése
            //_locationManager.RequestSingleUpdate(_locationProvider, this, null);

        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }
    }
}

