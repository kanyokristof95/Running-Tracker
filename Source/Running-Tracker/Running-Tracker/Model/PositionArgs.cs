using System;
using Running_Tracker.Persistence;

namespace Running_Tracker.Model
{
    /// <summary>
    /// Contains the current location.
    /// </summary>
    public class PositionArgs : EventArgs
    {
        public PositionArgs(LocationData locationData)
        {
            LocationData = locationData;
        }

        public LocationData LocationData { get; }
    }
}