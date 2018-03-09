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

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "HistoryActivity", Theme = "@style/MyTheme")]
    public class HistoryActivity : BaseActivity
    {
        private List<string> mItems;
        ArrayAdapter<string> adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.History);

            //Toolbar beállítása
            Android.Support.V7.Widget.Toolbar mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "History";

            //Listview beállítása
            ListView mListVIew = FindViewById<ListView>(Resource.Id.myListView);
            mItems = new List<string>
            {
                "First",
                "Second",
                "Third"
            };

            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            mListVIew.Adapter = adapter;
            mListVIew.ItemClick += MListVIew_ItemClick;
            mListVIew.ItemLongClick += MListVIew_ItemLongClick;

        }

        private void MListVIew_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Details");
            //TODO a futás adatainak kiírása a felugróablakba 
            alertDialog.SetMessage("TODO Running Details");

            alertDialog.SetPositiveButton("OK", delegate{});
            alertDialog.Show();
        }

        private void MListVIew_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //TODO A kiválasztott futás részletes megjelenítése
            var intent = new Intent(this, typeof(OldRunningActivity));
            /*példa, ha string paramétereket szeretnénk adni az activitynek
            intent.PutExtra("MyData", "Data from HistoryActivity");*/
            StartActivity(intent);
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