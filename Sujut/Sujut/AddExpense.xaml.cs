using System;
using System.Collections;
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
using Sujut.Helpers;

namespace Sujut
{
    public partial class AddExpense : PhoneApplicationPage
    {
        private Container _container;
        private DataServiceCollection<Participant> _participants;
        private long id;

        // Constructor
        public AddExpense()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (totalParticipants != 0) // Page already loaded
                return;

            id = long.Parse(NavigationContext.QueryString["debtCalculationId"]);

            _container = ApiHelper.GetContainer();

            _participants = new DataServiceCollection<Participant>(_container);

            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar
                {
                    BackgroundColor = Color.FromArgb(255, 0x86, 0xC4, 0x40), //"#86C440"
                    Opacity = 1,
                    ForegroundColor = Colors.White
                };

            ContentPanel.Children.Add(new ProgressBar
                {
                    IsIndeterminate = true,
                    Width = 300,
                    Margin = new Thickness(0, 30, 0, 0)
                });

            var query = _container.Participants
                                  .Expand(p => p.User)
                                  .Where(p => p.DebtCalculation.Id == id);

            _participants.LoadCompleted += ShowExpenseParticipants;
            _participants.LoadAsync(query);
        }
        
        private void ShowExpenseParticipants(object target, LoadCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            var participantsFromServer = _participants.Select(p => p.User).ToList();

            totalParticipants = participantsFromServer.Count;

            // Doing this because of the worthless WP8 binding of list texts
            // http://stackoverflow.com/questions/16642830/windows-phone-8-bind-to-string-resource-with-format
            foreach (var user in participantsFromServer)
            {
                user.Firstname = user.FullName();
            }

            var currentUser = participantsFromServer.Single(p => p.Id == ApiHelper.CurrentUserId());

            Payer.ItemsSource = participantsFromServer;
            Payer.SelectedItem = currentUser;

            Participants.SummaryForSelectedItemsDelegate = SummarizeItems;
            Participants.ItemsSource = participantsFromServer;
            Participants.SelectedItems = participantsFromServer;
        }

        private int totalParticipants;
        private string SummarizeItems(IList items)
        {
            return items.Count + " / " + totalParticipants;
        }

        private decimal amount;
        private string description;
        private User payer;
        private List<User> participants;
        private void Create_Click(object sender, EventArgs eventArgs)
        {
            if (!decimal.TryParse(Amount.Text, out amount))
            {
                MessageBox.Show(AppResources.AmountMustBeNumeric);
            }

            description = Description.Text;
            payer = (User)Payer.SelectedItem;

            participants = new List<User>();
            foreach (var participant in Participants.SelectedItems)
            {
                participants.Add((User)participant);
            }

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            var webClient = ApiHelper.AuthClient();
            webClient.UploadStringCompleted += ServerResponse;

            var json = JsonConvert.SerializeObject(new
                {
                    Amount = amount, Description = description, PayerId = payer.Id, DebtorIds = participants.Select(p => p.Id)
                });

            webClient.UploadStringAsync(ApiHelper.GetFullApiCallUri("api/DebtCalculations(" + id + ")/Expenses"), json);
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