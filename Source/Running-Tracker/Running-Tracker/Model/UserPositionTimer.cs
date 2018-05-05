using System;
using System.Timers;
using Android.Gms.Maps.Model;
using Running_Tracker.Persistence;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Animate the user position's circle.
    /// </summary>
    class UserPositionTimer
    {
        private Timer _timer;

        private int _count;
        private int _fps;
        private double _latDistance;
        private double _lngDistance;
        
        public UserPositionTimer(int fps, int gpsMinTime)
        {
            _count = 0;
            _fps = fps;

            var interval = (1000 / fps) * (1000.0 / gpsMinTime);
            _timer = new Timer {Interval = interval};
            _timer.Elapsed += TimerOnElapsed;
        }
        
        /// <summary>
        /// Current user's position
        /// </summary>
        public LatLng CurrentLatLng { get; private set; }

        /// <summary>
        /// Start animation.
        /// </summary>
        /// <param name="startLatLng">Start user's position</param>
        /// <param name="endLatLng">End user's position</param>
        public void Start(LatLng startLatLng, LatLng endLatLng)
        {
            _count = 0;
            CurrentLatLng = startLatLng;

            _latDistance = (endLatLng.Latitude - startLatLng.Latitude) / _fps;
            _lngDistance = (endLatLng.Longitude - startLatLng.Longitude) / _fps;
            
            if(_timer.Enabled)
                _timer.Stop();

            _timer.Start();
        }

        /// <summary>
        /// One frame of animated user's location
        /// </summary>
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_count < _fps)
            {
                CurrentLatLng.Longitude += _lngDistance;
                CurrentLatLng.Latitude += _latDistance;

                OnElapsed(new LocationData(CurrentLatLng.Longitude, CurrentLatLng.Latitude));
                _count++;
            }
            else
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Event handler for elapsed.
        /// </summary>
        public event EventHandler<PositionArgs> Elapsed;
        
        /// <summary>
        /// Sends the Elapsed signal.
        /// </summary>
        /// <param name="location">Animated location</param>
        private void OnElapsed(LocationData location)
        {
            Elapsed?.Invoke(this, new PositionArgs(location));
        }
    }
}