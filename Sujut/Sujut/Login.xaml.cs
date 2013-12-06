using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sujut.Api;
using Sujut.Core;
using Sujut.Resources;
using Sujut.SujutApi;

namespace Sujut
{
    public partial class Login : PhoneApplicationPage
    {
        // Constructor
        public Login()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiHelper.UserIsLoggedIn())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private string email;
        private string password;
        private void Login_Click(object sender, EventArgs eventArgs)
        {
            email = Email.Text;
            password = Password.Password;

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            // Check with server that credentials are OK.
            var webClient = new WebClient();
            webClient.UploadStringCompleted += LoginValidated;
            webClient.Headers["Content-Type"] = "application/json";
            webClient.UploadStringAsync(ApiHelper.GetFullApiCallUri("api/Users/Validate"),
                                        JsonConvert.SerializeObject(new {Email = email, Password = password}));
        }

        private void LoginValidated(object target, UploadStringCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            dynamic result = JsonConvert.DeserializeObject(eventArgs.Result);
            var userId = long.Parse(result.value.Value);

            if (userId > 0)
            {
                ApiHelper.SaveCredentials(email, password);
                ApiHelper.SaveCurrentUserId(userId);

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                // Show failure message
            }
        }
    }
}