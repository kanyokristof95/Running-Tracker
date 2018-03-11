using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Running_Tracker.Persistence
{
    public class RunningData
    {
        private List<LocationData> locationList;
        private PersonalDatas personalDatas;
        private bool open;
        private double distance;

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
                throw new NotImplementedException();
            }
        }

        public RunningData(PersonalDatas personalDatas)
        {
            open = true;
            distance = 0;

            this.personalDatas = personalDatas;
            locationList = new List<LocationData>();
            StartDateTime = DateTime.Now;
        }

        public void Add(LocationData location)
        {
            if (open)
            {
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
    }
}