using System;
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

namespace SpecialToolbarApp
{
    [Activity(Label = "HistoryActivity", Theme = "@style/MyTheme")]
    public class HistoryActivity : ActionBarActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.History);

            // Create your application here
            var mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "History";

            var listView = FindViewById<ListView>(Resource.Id.MyListView);

            List<string> list = new List<string>();
            list.Add("First");
            list.Add("Second");
            list.Add("Third");
            list.Add("Asdfsfs");
            list.Add("Bdgdfgdf");
            list.Add("Cssdf");

            var ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleExpandableListItem1, list);

            listView.Adapter = ListAdapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.undo_menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.undo)
            {
                base.OnBackPressed();
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}