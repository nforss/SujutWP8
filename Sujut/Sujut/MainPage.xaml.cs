using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sujut.Api;
using Sujut.Core;
using Sujut.Resources;
using Sujut.SujutApi;

namespace Sujut
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Container _container;
        private DataServiceCollection<DebtCalculation> _calculations;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            _container = ApiHelper.GetContainer();
            _calculations = new DataServiceCollection<DebtCalculation>(_container);

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
            
            var query = _container.DebtCalculations.OrderByDescending(dc => dc.LastActivityTime);
            
            _calculations.LoadCompleted += BuildButtonList;
            _calculations.LoadAsync(query);
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

        private void BuildButtonList(object target, LoadCompletedEventArgs eventArgs)
        {
            if (eventArgs.Error != null)
            {
                // Show error message
            }
            else
            {
                ButtonList.Children.Clear();

                var calcs = _calculations;

                foreach (var calc in calcs)
                {
                    var grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());

                    var nameBlock = new TextBlock {Text = calc.Name, HorizontalAlignment = HorizontalAlignment.Center};
                    Grid.SetRow(nameBlock, 0);
                    grid.Children.Add(nameBlock);

                    if (calc.LastActivityTime != DateTime.MinValue)
                    {
                        var dateBlock = new TextBlock
                            {
                                Text =
                                    (calc.LastActivityTime).ToShortDateString() + " " +
                                    (calc.LastActivityTime).ToShortTimeString(),
                                FontSize = 15,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                        Grid.SetRow(dateBlock, 1);
                        grid.Children.Add(dateBlock);
                    }

                    var button = new Button
                        {
                            Content = grid,
                            DataContext = new {DebtCalculationId = calc.Id},
                            Foreground = (SolidColorBrush) Application.Current.Resources["BlackAccentBrush"],
                            BorderBrush = (SolidColorBrush) Application.Current.Resources["BlackAccentBrush"]
                        };

                    button.Click += Button_Click;

                    ButtonList.Children.Add(button);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dynamic idData = ((Button)sender).DataContext;
            NavigationService.Navigate(new Uri("/ShowDebtCalculation.xaml?debtCalculationId=" + idData.DebtCalculationId, UriKind.Relative));
        }
    }
}