using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Running_Tracker.Persistence;

namespace Running_Tracker.ViewActivity
{
    [Activity(Label = "HistoryActivity", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HistoryActivity : BaseActivity
    {
        private List<RunningData> _mItems;
        private ListView _mListVIew;
        private ArrayAdapter<RunningData> _adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.History);

            // Toolbar
            var mToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "History";

            // Listview
            _mListVIew = FindViewById<ListView>(Resource.Id.myListView);
            _mListVIew.ItemClick += MListVIew_ItemClick;
        }
        
        private void MListVIew_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(OldRunningActivity));
            intent.PutExtra("Running", JsonConvert.SerializeObject(_mItems[e.Position]));
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
                OnBackPressed();
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnResume()
        {
            base.OnResume();

            _mItems = model.LoadPreviousRunnings();
            _adapter = new ArrayAdapter<RunningData>(this, Android.Resource.Layout.SimpleListItem1, _mItems);
            _mListVIew.Adapter = _adapter;
        }
    }
}