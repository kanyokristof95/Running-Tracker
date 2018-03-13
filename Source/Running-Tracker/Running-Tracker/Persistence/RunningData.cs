using System;
using System.Collections.Generic;

namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Contains the running datas.
    /// </summary>
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

        /// <summary>
        /// Locations of the running.
        /// </summary>
        public List<LocationData> Locations
        {
            get { return locationList; }
        }

        /// <summary>
        /// The running's distance in meter.
        /// </summary>
        public double Distance
        {
            get { return distance; }
        }

        /// <summary>
        /// The start date of running.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// The end date of running.
        /// </summary>
        public DateTime EndDateTime { get; private set; }

        /// <summary>
        /// The duration of running.
        /// </summary>
        public TimeSpan Duration
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
        /// The average speed of the running in km/h
        /// </summary>
        public double AverageSpeed
        {
            get { return (Distance / 1000) / (Duration.TotalHours); }
        }

        /// <summary>
        /// The vertical up distance of running in meter
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
        /// The vertical down distance of running in meter
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

        /// <summary>
        /// The burned calorie during the running
        /// </summary>
        public double Calorie
        {
            get
            {
                double calcCoefficient = 0.790; // Running - 0.790, Biking - 0.28, Swimming - 2.93
                return calcCoefficient * personalDatas.Weight * 2.2046 * Distance / 1609.344;
            }
        }

        /// <summary>
        /// The min longitude of running
        /// </summary>
        public double MinLongitude
        {
            get { return minLongitude; }
        }

        /// <summary>
        /// The max langitude of running
        /// </summary>
        public double MaxLongitude
        {
            get { return maxLongitude; }
        }

        /// <summary>
        /// The min latitude of running
        /// </summary>
        public double MinLatitude
        {
            get { return minLatitude; }
        }

        /// <summary>
        /// The max latitude of running
        /// </summary>
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

        /// <summary>
        /// Add the new location to the list of locations
        /// </summary>
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

        /// <summary>
        /// Finish the running. After it you cannot add new points.
        /// </summary>
        public void Finish()
        {
            if (open)
            {
                EndDateTime = DateTime.Now;
                open = false;
            }
        }

        /// <summary>
        /// Convert the RunningData into string.
        /// </summary>
        /// <returns>StartDateTime in (yyyy. MM. dd. HH:mm:ss) format</returns>
        public override string ToString()
        {
            return StartDateTime.ToString("yyyy. MM. dd. HH:mm:ss");
        }
    }
}