using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sujut.Resources;

namespace Sujut
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar();
            var createNewButton = new ApplicationBarIconButton
            {
                Text = AppResources.CreateNew,
                IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative)
            };

            createNewButton.Click += CreateNew_Click;
            ApplicationBar.Buttons.Add(createNewButton);

            var syncButton = new ApplicationBarIconButton
            {
                Text = AppResources.Sync,
                IconUri = new Uri("/Assets/Icons/sync.png", UriKind.Relative)
            };

            createNewButton.Click += Sync_Click;
            ApplicationBar.Buttons.Add(syncButton);

            ShowDebtCalculations();
        }

        private void CreateNew_Click(object sender, EventArgs eventArgs)
        {
            NavigationService.Navigate(new Uri("/CreateNew.xaml", UriKind.Relative));
        }

        private void Sync_Click(object sender, EventArgs eventArgs)
        {
            // sync
        }

        private void ShowDebtCalculations()
        {
            // Get calculations from isolated storage.
        }
    }
}