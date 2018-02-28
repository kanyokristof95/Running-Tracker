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

namespace View
{
    [Activity(Label = "BasicListView")]
    public class BasicListView : Activity
    {
        private List<string> mItems;
        ArrayAdapter<string> adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BasicListView);

           ListView mListVIew = FindViewById<ListView>(Resource.Id.myListView);

            mItems = new List<string>();
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");
            mItems.Add("Go to first page");
            mItems.Add("Go to second page");


            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);

            mListVIew.Adapter = adapter;

            mListVIew.ItemClick += MListVIew_ItemClick;
            mListVIew.ItemLongClick += MListVIew_ItemLongClick;

        }

        private void MListVIew_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            StartActivity(typeof(ComplexLayout));
        }

        private void MListVIew_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(e.Position % 2 == 1)
            {
                StartActivity(typeof(second));
            }
            else
            {
                StartActivity(typeof(MainActivity));
            } 
        }
    }
}