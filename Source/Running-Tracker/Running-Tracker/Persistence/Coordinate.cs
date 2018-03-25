namespace Running_Tracker.Persistence
{
    public class Position
    {
        /// <summary>
        /// Latitude of position
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of position
        /// </summary>
        public double Longitude { get; set; }

        public Position(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}