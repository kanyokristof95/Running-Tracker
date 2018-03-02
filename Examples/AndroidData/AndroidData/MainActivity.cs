using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AndroidData
{
    [Activity(Label = "AndroidData", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button submitButton = FindViewById<Button>(Resource.Id.SubmitButton);
            submitButton.Click += SubmitButton_Click;

            Button viewContactButton = FindViewById<Button>(Resource.Id.viewContactButton);
            viewContactButton.Click += ViewContactButton_Click;
        }
        
        private void SubmitButton_Click(object sender, System.EventArgs e)
        {
            // Get data
            EditText nameBox = FindViewById<EditText>(Resource.Id.nameBox);
            string name = nameBox.Text;
            EditText phoneBox = FindViewById<EditText>(Resource.Id.phoneBox);
            string phone = phoneBox.Text;

            Contact contact = new Contact(name, phone);

            // Add the new contact to the share preferences
            var localContacts = Application.Context.GetSharedPreferences("MyContacts", FileCreationMode.Private);
            var contactEdit = localContacts.Edit();

            List<string> list = new List<string>();
            ICollection<string> collection = localContacts.GetStringSet("ContactList", null);

            if (collection != null) {
                list = collection.ToList();
            }
            
            list.Add(JsonConvert.SerializeObject(contact));
            
            contactEdit.PutStringSet("ContactList", list);

            contactEdit.Commit();

            // Create a toast notification
            Android.Widget.Toast.MakeText(this, "Item Added", ToastLength.Short).Show();

            // Cleat boxes
            nameBox.Text = "";
            phoneBox.Text = "";

        }
        
        private void ViewContactButton_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(ViewContactsActivity));
            StartActivity(intent);
        }
    }
}

