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
using Sujut.Helpers;
using Sujut.Resources;
using Sujut.SujutApi;

namespace Sujut
{
    public partial class NewCalculation : PhoneApplicationPage
    {
        // Constructor
        public NewCalculation()
        {
            InitializeComponent();

            var currencies = CultureHelper.GetCurrencies();

            foreach (var currencyAlternative in currencies)
            {
                Currency.Items.Add(currencyAlternative.Key);
            }
        }
        
        private string name;
        private string description;
        private string currency;
        private void Create_Click(object sender, EventArgs eventArgs)
        {
            name = Name.Text;
            description = Description.Text;
            currency = Currency.SelectedItem.ToString();

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            var webClient = ApiHelper.AuthClient();
            webClient.UploadStringCompleted += ServerResponse;

            var json = JsonConvert.SerializeObject(new { Name = name, Description = description, Currency = currency }, new JsonStringConverter());
            webClient.UploadStringAsync(ApiHelper.GetFullApiCallUri("api/DebtCalculations"), json);
        }

        private void ServerResponse(object target, UploadStringCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            if (eventArgs.Error != null)
            {
                MessageBox.Show(AppResources.ErrorProcessingRequest);
            }
            else
            {
                dynamic result = JsonConvert.DeserializeObject(eventArgs.Result);
                var calculationId = long.Parse(result.value.Value);

                NavigationService.Navigate(new Uri("/ShowDebtCalculation.xaml?debtCalculationId=" + calculationId, UriKind.Relative));
            }
        }
    }
}