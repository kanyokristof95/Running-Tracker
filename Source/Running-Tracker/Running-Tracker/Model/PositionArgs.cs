using System;
using Running_Tracker.Persistence;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the current location.
    /// </summary>
    public class PositionArgs : EventArgs
    {
        private LocationData locationData;
        
        public PositionArgs(LocationData locationData)
        {
            this.locationData = locationData;
        }

        public LocationData LocationData
        {
            get { return locationData; }
        }
    }
}