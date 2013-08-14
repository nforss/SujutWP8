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
    public partial class MainPage : PhoneApplicationPage
    {
        private string json =
            "[{\"Id\":1,\"Name\":\"Saunominen Herttoniemessä\",\"Phase\":\"CollectingExpenses\"},{\"Id\":5,\"Name\":\"Testing\",\"Phase\":\"CollectingPayments\"},{\"Id\":6,\"Name\":\"Barbie at the Shomaker's Village\",\"Phase\":\"Canceled\"},{\"Id\":7,\"Name\":\"Barbie at the Shomaker's Village\",\"Phase\":\"GatheringParticipants\"},{\"Id\":9,\"Name\":\"Barbie at the Shomaker's Village\",\"Phase\":\"GatheringParticipants\"},{\"Id\":10,\"Name\":\"Barbie at the Shomaker's Village\",\"Phase\":\"GatheringParticipants\"},{\"Id\":11,\"Name\":\"Blabla\",\"Phase\":\"CollectingExpenses\"},{\"Id\":12,\"Name\":\"Testing\",\"Phase\":\"CollectingPayments\"},{\"Id\":13,\"Name\":\"mailtest\",\"Phase\":\"CollectingExpenses\"},{\"Id\":14,\"Name\":\"mail 2\",\"Phase\":\"CollectingExpenses\"},{\"Id\":15,\"Name\":\"Currency test\",\"Phase\":\"CollectingExpenses\"},{\"Id\":16,\"Name\":\"Concurrency\",\"Phase\":\"CollectingExpenses\"},{\"Id\":17,\"Name\":\"jeee\",\"Phase\":\"CollectingExpenses\"},{\"Id\":18,\"Name\":\"bleh\",\"Phase\":\"GatheringParticipants\"},{\"Id\":19,\"Name\":\"New controller methods\",\"Phase\":\"CollectingExpenses\"}]";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            ShowDebtCalculations();
        }

        private void ShowDebtCalculations()
        {
            ButtonList.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar
                {
                    BackgroundColor = Color.FromArgb(255, 0x86, 0xC4, 0x40), //"#86C440"
                    Opacity = 1,
                    ForegroundColor = Colors.White
                }; 
            var createNewButton = new ApplicationBarIconButton
            {
                Text = AppResources.CreateNew,
                IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative)
            };

            createNewButton.Click += CreateNew_Click;
            ApplicationBar.Buttons.Add(createNewButton);

            var syncButton = new ApplicationBarIconButton
            {
                Text = AppResources.Logout,
                IconUri = new Uri("/Assets/Icons/close.png", UriKind.Relative)
            };

            createNewButton.Click += Logout_Click;
            ApplicationBar.Buttons.Add(syncButton);

            var webClient = ApiHelper.AuthClient();
            webClient.DownloadStringCompleted += BuildButtonList;
            webClient.DownloadStringAsync(ApiHelper.GetFullApiCallUri("api/debtcalculation/all/"));

            //BuildButtonList(null, null);
        }

        private void CreateNew_Click(object sender, EventArgs eventArgs)
        {
            NavigationService.Navigate(new Uri("/CreateNew.xaml", UriKind.Relative));
        }

        private void Logout_Click(object sender, EventArgs eventArgs)
        {
            ApiHelper.Logout();

            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void BuildButtonList(object target, DownloadStringCompletedEventArgs eventArgs)
        {
            ButtonList.Children.Clear();

            var calcs = EntityCreator.DebtCalculationsFromJson(eventArgs.Result);
            //var calcs = EntityCreator.DebtCalculationsFromJson(json);

            foreach (var calc in calcs.OrderByDescending(calc => calc.LastActivityTime))
            {
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                var nameBlock = new TextBlock { Text = calc.Name, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(nameBlock, 0);
                grid.Children.Add(nameBlock);

                if (calc.LastActivityTime != null)
                {
                    var dateBlock = new TextBlock
                    {
                        Text = ((DateTime)calc.LastActivityTime).ToShortDateString() + " " + ((DateTime)calc.LastActivityTime).ToShortTimeString(),
                        FontSize = 15,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    Grid.SetRow(dateBlock, 1);
                    grid.Children.Add(dateBlock);
                }

                var button = new Button
                    {
                        Content = grid, 
                        DataContext = new { DebtCalculationId = calc.Id },
                        Background = (SolidColorBrush)Application.Current.Resources["SujutFooterAccentBrush"],
                        Foreground = (SolidColorBrush)Application.Current.Resources["BlackAccentBrush"],
                        BorderBrush = (SolidColorBrush)Application.Current.Resources["BlackAccentBrush"]
                    };

                button.Click += Button_Click;

                ButtonList.Children.Add(button);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dynamic idData = ((Button)sender).DataContext;
            NavigationService.Navigate(new Uri("/ShowDebtCalculation.xaml?debtCalculationId=" + idData.DebtCalculationId, UriKind.Relative));
        }
    }
}