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
            if (ApiHelper.UserIsLoggedIn())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

            InitializeComponent();
        }
        
        private void Login_Click(object sender, EventArgs eventArgs)
        {
            var email = Email.Text;
            var passwd = Password.Password;

            // Check with server that credentials are OK.

            ApiHelper.SaveCredentials(email, passwd);

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}