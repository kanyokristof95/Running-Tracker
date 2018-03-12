using System;
using System.Collections.Generic;

namespace Running_Tracker.Persistence
{
    public class RunningData
    {
        private List<LocationData> locationList;
        private PersonalDatas personalDatas;
        private bool open;
        private double distance;

        private double minLongitude;
        private double maxLongitude;
        private double minLatitude;
        private double maxLatitude;

        public List<LocationData> Locations
        {
            get { return locationList; }
        }

        /// <summary>
        /// In meter
        /// </summary>
        public double Distance
        {
            get { return distance; }
        }

        public DateTime StartDateTime { get; private set; }

        public DateTime EndDateTime { get; private set; }

        public TimeSpan Time
        {
            get
            {
                if(open)
                {
                    return DateTime.Now - StartDateTime;
                } else
                {
                    return EndDateTime - StartDateTime;
                }
            }
        }

        /// <summary>
        /// In km/hour
        /// </summary>
        public double AverageSpeed
        {
            get { return (Distance / 1000) / (Time.TotalHours); }
        }

        /// <summary>
        /// In meter
        /// </summary>
        public double Up
        {
            get
            {
                double temp = 0;

                foreach (LocationData data in locationList)
                {
                    temp += data.Up;
                }

                return temp;
            }
        }

        /// <summary>
        /// In meter
        /// </summary>
        public double Down
        {
            get
            {
                double temp = 0;

                foreach (LocationData data in locationList)
                {
                    temp += data.Down;
                }

                return temp;
            }
        }

        public double Calorie
        {
            get
            {
                return 0; // TODO
            }
        }

        public double MinLongitude
        {
            get { return minLongitude; }
        }

        public double MaxLongitude
        {
            get { return maxLongitude; }
        }

        public double MinLatitude
        {
            get { return minLatitude; }
        }

        public double MaxLatitude
        {
            get { return maxLatitude; }
        }

        public RunningData(PersonalDatas personalDatas)
        {
            open = true;
            distance = 0;

            minLongitude = double.PositiveInfinity;
            maxLongitude = double.NegativeInfinity;
            minLatitude = double.PositiveInfinity;
            maxLatitude = double.NegativeInfinity;

            this.personalDatas = personalDatas;
            locationList = new List<LocationData>();
            StartDateTime = DateTime.Now;
        }

        public void Add(LocationData location)
        {
            if (open)
            {
                if (location.Longitude < minLongitude)
                    minLongitude = location.Longitude;

                if (location.Longitude > maxLongitude)
                    maxLongitude = location.Longitude;

                if (location.Latitude < minLatitude)
                    minLatitude = location.Latitude;

                if (location.Latitude > maxLatitude)
                    maxLatitude = location.Latitude;

                distance += location.Distance;
                locationList.Add(location);
            }
        }

        public void Finish()
        {
            if (open)
            {
                EndDateTime = DateTime.Now;
                open = false;
            }
        }

        public override string ToString()
        {
            return StartDateTime.ToString("yyyy. MM. dd. HH:mm:ss");
        }
    }
}