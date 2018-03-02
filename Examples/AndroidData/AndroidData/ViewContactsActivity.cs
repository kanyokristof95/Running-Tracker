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
using Newtonsoft.Json;

namespace AndroidData
{
    [Activity(Label = "ViewContactsActivity")]
    public class ViewContactsActivity : Activity
    {
        ListView listView;
        ArrayAdapter<Contact> ListAdapter;

        List<Contact> contactList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewContacts);

            listView = FindViewById<ListView>(Resource.Id.contentListView);

            contactList = new List<Contact>();
            var localContacts = Application.Context.GetSharedPreferences("MyContacts", FileCreationMode.Private);

            ICollection<string> collection = localContacts.GetStringSet("ContactList", null);
            if (collection != null)
            {
                List<string> contactStringList = collection.ToList();

                foreach (string contactString in contactStringList)
                {
                    Contact contact = JsonConvert.DeserializeObject<Contact>(contactString);
                    contactList.Add(contact);
                }
            }

            // Add the list to the list adapter
            contactList.Sort((a, b) => string.Compare(a.Name, b.Name));
            ListAdapter = new ArrayAdapter<Contact>(this, Android.Resource.Layout.SimpleExpandableListItem1, contactList);

            listView.Adapter = ListAdapter;
            listView.ItemClick += ListView_ItemClick;
            listView.ItemLongClick += ListView_ItemLongClick;
        }
        
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("TODO");
            alertDialog.SetMessage("TODO");
            alertDialog.Show();
        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var item = contactList[e.Position];
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("Contact");
            alertDialog.SetMessage("Name: " + item.Name + "\nPhone number: " + item.PhoneNumber);
            alertDialog.SetNegativeButton("Delete", delegate
            {
                contactList.Remove(item);
                ListAdapter = new ArrayAdapter<Contact>(this, Android.Resource.Layout.SimpleExpandableListItem1, contactList);


                var localContacts = Application.Context.GetSharedPreferences("MyContacts", FileCreationMode.Private);
                var contactEdit = localContacts.Edit();

                List<string> list = new List<string>();

                foreach (Contact contact in contactList)
                {
                    list.Add(JsonConvert.SerializeObject(contact));
                }

                contactEdit.PutStringSet("ContactList", list);

                contactEdit.Commit();
            });

            alertDialog.SetPositiveButton("OK", delegate
            {
                // SKIP
            });
            alertDialog.Show();
        }
    }
}