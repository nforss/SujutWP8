using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sujut.Api;
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
            var webClient = ApiHelper.AuthClient();
            webClient.DownloadStringCompleted += BuildButtonList;
            webClient.DownloadStringAsync(ApiHelper.GetFullApiCallUri("api/event/all/"));
        }

        private void BuildButtonList(object target, DownloadStringCompletedEventArgs eventArgs)
        {
            ButtonList.Children.Clear();

            var buttons = ApiHelpers.EventListFromJson(eventArgs.Result);

            foreach (var kvp in buttons.OrderByDescending(kv => kv.Key))
            {
                kvp.Value.Click += Button_Click;
                ButtonList.Children.Add(kvp.Value);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dynamic guidData = ((Button)sender).DataContext;
            NavigationService.Navigate(new Uri("/EventView.xaml?venueGuid=" + guidData.VenueGuid + "&eventGuid=" + guidData.EventGuid, UriKind.Relative));
        }
    }
}