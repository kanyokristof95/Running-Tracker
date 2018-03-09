﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Running_Tracker
{
    [Activity(Label = "OldRunningActivity", Theme = "@style/MyTheme")]
    public class OldRunningActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Run details";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.old_running_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.undo:
                    base.OnBackPressed();
                    break;
                case Resource.Id.delete:
                    //TODO felugróablak megerősítést kér a törlésről, majd megerősítés esetén a törlés elvégzése
                    base.OnBackPressed();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}