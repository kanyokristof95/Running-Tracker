using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Contains the running datas.
    /// </summary>
    public class RunningData
    {
        /// <summary>
        /// Locations of the running.
        /// </summary>
        public List<LocationData> Locations { get; set; }

        /// <summary>
        /// Locations of running's stops
        /// </summary>
        public List<LatLng> Stops { get; set; }
        
        /// <summary>
        /// The runner's personal datas
        /// </summary>
        public PersonalData PersonalInformation { get; set; }
        
        /// <summary>
        /// Is the running open
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// The running's distance in meter.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The start date of running.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// The end date of running.
        /// </summary>
        public DateTime EndDateTime { get; set; }
        
        /// <summary>
        /// The min longitude of running
        /// </summary>
        public double MinLongitude { get; set; }

        /// <summary>
        /// The max langitude of running
        /// </summary>
        public double MaxLongitude { get; set; }

        /// <summary>
        /// The min latitude of running
        /// </summary>
        public double MinLatitude { get; set; }

        /// <summary>
        /// The max latitude of running
        /// </summary>
        public double MaxLatitude { get; set; }

        /// <summary>
        /// The duration of running.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                if (Open)
                {
                    return DateTime.Now - StartDateTime;
                }
                else
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

                foreach (LocationData data in Locations)
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

                foreach (LocationData data in Locations)
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
                return calcCoefficient * PersonalInformation.Weight * 2.2046 * Distance / 1609.344;
            }
        }

        public RunningData(PersonalData personalDatas)
        {
            Open = true;
            Distance = 0;

            MinLongitude = double.PositiveInfinity;
            MaxLongitude = double.NegativeInfinity;
            MinLatitude = double.PositiveInfinity;
            MaxLatitude = double.NegativeInfinity;

            PersonalInformation = personalDatas;
            Locations = new List<LocationData>();
            Stops = new List<LatLng>();
            StartDateTime = DateTime.Now;
        }

        /// <summary>
        /// Add the new location to the list of locations
        /// </summary>
        public void AddLocation(LocationData location)
        {
            if (Open)
            {
                if (location.Longitude < MinLongitude)
                    MinLongitude = location.Longitude;

                if (location.Longitude > MaxLongitude)
                    MaxLongitude = location.Longitude;

                if (location.Latitude < MinLatitude)
                    MinLatitude = location.Latitude;

                if (location.Latitude > MaxLatitude)
                    MaxLatitude = location.Latitude;

                Distance += location.Distance;
                Locations.Add(location);
            }
        }

        /// <summary>
        /// Add a new stop for the running
        /// </summary>
        /// <param name="location"></param>
        public void AddStop(LatLng location)
        {
            if (Open)
                Stops.Add(location);
        }

        /// <summary>
        /// Finish the running. After it you cannot add new points.
        /// </summary>
        public void Finish()
        {
            if (Open)
            {
                EndDateTime = DateTime.Now;
                Open = false;
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