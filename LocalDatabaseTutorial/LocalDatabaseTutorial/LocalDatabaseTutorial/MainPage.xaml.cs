using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http;
using System.Net;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.IO;
using SQLite;


namespace LocalDatabaseTutorial
{

    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            getDataFromDatBseAsync();

            const double interval6Minutes = 6 * 6 * 1000; // milliseconds to one hour

            Timer checkForTime = new Timer(interval6Minutes);
            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            checkForTime.Enabled = true;
        }
        //Function For checking time period
        void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {

        }
        //function for brening data from Local SqLiteDb
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await App.Database.GetPeopleASync();
            var data1 = App.Database.GetPeopleASync();


            //Json Convertion
            string JSONString = string.Empty;
            var personJson = JsonConvert.SerializeObject(data1.Result);


            CheckForInternetConnection(personJson);

        }
        //Saving Data
        async void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameEntry.Text) && !string.IsNullOrWhiteSpace(ageEntry.Text))
            {
                await App.Database.savePersonAsync(new Person
                {
                    Name = nameEntry.Text,
                    Age = int.Parse(ageEntry.Text)
                });

                nameEntry.Text = ageEntry.Text = string.Empty;
                listView.ItemsSource = await App.Database.GetPeopleASync();
            }
        }
        //Posting Data To SQL Server(Using API) 
        public static void SubmitPersonToAPIAsync(string personJson)
        {

            HttpClient httpClient = new HttpClient();

            HttpContent stringContent = new StringContent(personJson, Encoding.UTF8, "application/json");
            try
            {
                //var data = await httpClient.GetAsync("http://localhost:44386/api/values");
                var result = httpClient.PostAsync("http://localhost:44386/api/values", stringContent);

            }
            catch (Exception e)
            {

            }


        }
        //For Checking Server Is Availabe or not 
        public static void CheckForInternetConnection(string personJson)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://localhost:44386"))
                {
                    SubmitPersonToAPIAsync(personJson);


                }
            }
            catch
            {

            }
        }



        ///for geting data from the sql
        public static async Task getDataFromDatBseAsync()
        {

            HttpClient httpClient = new HttpClient();

            //HttpContent stringContent = new StringContent(Database, Encoding.UTF8, "application/json");
            try
            {
                //var data = await httpClient.GetAsync("http://localhost:44386/api/values");
                var url = "http://localhost:44386/api/values";//Paste ur url here  

                WebRequest request = HttpWebRequest.Create(url);


                WebResponse response = request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string responseText = reader.ReadToEnd();

                // var account = JsonConvert.DeserializeObject(responseText);

                var product = JsonConvert.DeserializeObject<List<Person>>(responseText);

                await App.Database.Post(product);


                //listView.ItemsSource = await App.Database.GetPeopleASync();

            }
            catch (Exception e)
            {

            }


        }
        // for implementing notification using firebase
        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                var FCMToken = Application.Current.Properties.Keys.Contains("Fcmtocken");
                if (FCMToken)
                {
                    var FCMTockenValue = Application.Current.Properties["Fcmtocken"].ToString();
                    FCMBody body = new FCMBody();
                    FCMNotification notification = new FCMNotification();
                    notification.title = "Xamarin Forms FCM Notifications";
                    notification.body = "Sample For FCM Push Notifications in Xamairn Forms";
                    FCMData data = new FCMData();
                    data.key1 = "";
                    data.key2 = "";
                    data.key3 = "";
                    data.key4 = "";
                    body.registration_ids = new[] { FCMTockenValue };
                    body.notification = notification;
                    body.data = data;
                    var isSuccessCall = SendNotification(body).Result;
                    if (isSuccessCall)
                    {
                        DisplayAlert("Alart", "Notifications Send Successfully", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Alart", "Notifications Send Failed", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<bool> SendNotification(FCMBody fcmBody)
        {
            try
            {
                var httpContent = JsonConvert.SerializeObject(fcmBody);
                var client = new HttpClient();
                var authorization = string.Format("key={0}", "AAAAdo7memY:APA91bHNfseEKErXXXXXXX");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
                var stringContent = new StringContent(httpContent);
                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string uri = "https://fcm.googleapis.com/fcm/send";
                var response = await client.PostAsync(uri, stringContent).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TaskCanceledException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
