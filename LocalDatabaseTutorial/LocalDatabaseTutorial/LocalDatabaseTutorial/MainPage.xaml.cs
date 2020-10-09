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
        public static  void SubmitPersonToAPIAsync(string personJson)
        {
            
            HttpClient httpClient = new HttpClient();

            HttpContent stringContent = new StringContent(personJson, Encoding.UTF8, "application/json");
            try
            {
                //var data = await httpClient.GetAsync("http://localhost:44386/api/values");
                var result =  httpClient.PostAsync("http://localhost:44386/api/values", stringContent);
               
            }
            catch(Exception e)
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

    }
}
