﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Running_Tracker.Persistence
{
    class RunningTrackerDataAccess : IRunningTrackerDataAccess
    {
        public RunningTrackerDataAccess() {}
        
        public List<RunningData> LoadPreviousRunnings()
        {
            ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);

            ICollection<string> stringICollection = runningTracker.GetStringSet("Runnings", null);
            List<RunningData> runnings = new List<RunningData>();

            if(stringICollection != null)
            {
                List<string> stringList = stringICollection.ToList();

                foreach(string element in stringList)
                {
                    runnings.Add(JsonConvert.DeserializeObject<RunningData>(element));
                }
            }
            runnings.Sort((a, b) => DateTime.Compare(a.StartDateTime, b.StartDateTime));

            return runnings;
        }

        public void SaveRunning(RunningData running)
        {
            ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);

            ICollection<string> stringICollection = runningTracker.GetStringSet("Runnings", null);
            List<string> runningsString = new List<string>();

            if (stringICollection != null)
            {
                runningsString = stringICollection.ToList();
            }

            runningsString.Add(JsonConvert.SerializeObject(running));

            ISharedPreferencesEditor runningTrackerEditor = runningTracker.Edit();
            runningTrackerEditor.PutStringSet("Runnings", runningsString);
            runningTrackerEditor.Commit();
        }

        public void DeleteRunning(RunningData running)
        {
            ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);

            ICollection<string> stringICollection = runningTracker.GetStringSet("Runnings", null);
            List<string> runningsString = new List<string>();

            if (stringICollection != null)
            {
                runningsString = stringICollection.ToList();
            }

            runningsString.Remove(JsonConvert.SerializeObject(running));

            ISharedPreferencesEditor runningTrackerEditor = runningTracker.Edit();
            runningTrackerEditor.PutStringSet("Runnings", runningsString);
            runningTrackerEditor.Commit();
        }
        
        public PersonalDatas CurrentPersonalDatas
        {
            get
            {
                ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);
                string personalDatasString = runningTracker.GetString("PersonalDatas", null);

                PersonalDatas personalDatas = new PersonalDatas();

                if (personalDatasString != null)
                {
                    personalDatas = JsonConvert.DeserializeObject<PersonalDatas>(personalDatasString);
                }

                return personalDatas;
            }

            set
            {
                ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);
                ISharedPreferencesEditor runningTrackerEditor = runningTracker.Edit();

                string dataString = JsonConvert.SerializeObject(value);
                
                runningTrackerEditor.PutString("PersonalDatas", dataString);
                runningTrackerEditor.Commit();
            }
        }

        public WarningValues CurrentWarningValues
        {
            get
            {
                ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);
                string warningValuesString = runningTracker.GetString("WarningValues", null);

                WarningValues warningValues = new WarningValues();

                if (warningValuesString != null)
                {
                    warningValues = JsonConvert.DeserializeObject<WarningValues>(warningValuesString);
                }

                return warningValues;
            }

            set
            {
                ISharedPreferences runningTracker = Application.Context.GetSharedPreferences("RunningTracker", FileCreationMode.Private);
                ISharedPreferencesEditor runningTrackerEditor = runningTracker.Edit();

                string dataString = JsonConvert.SerializeObject(value);

                runningTrackerEditor.PutString("WarningValues", dataString);
                runningTrackerEditor.Commit();
            }
        }
    }
}