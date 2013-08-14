using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using Sujut.Api;
using Sujut.Core;
using Sujut.Resources;

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
            webClient.DownloadStringCompleted += Login_Click;
            webClient.DownloadStringAsync(ApiHelper.GetFullApiCallUri("api/validateuser/" + email + "/" + password));
        }

        private void Login_Click(object target, DownloadStringCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            var validationSucceeded = bool.Parse(eventArgs.Result);

            if (validationSucceeded)
            {
                ApiHelper.SaveCredentials(email, password);

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                // Show failure message
            }
        }
    }
}