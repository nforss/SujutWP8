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
    public partial class ShowDebtCalculation : PhoneApplicationPage
    {
        private string json = "{\"Id\":12,\"Name\":\"Testing\",\"Description\":null,\"Currency\":\"EUR\",\"LastActivityTime\":\"2013-05-27T15:02:43\",\"Phase\":3,\"CreatorId\":0,\"Participants\":[{\"Id\":1,\"Email\":\"nicklas.forss@iki.fi\",\"Firstname\":\"Nicklas\",\"Lastname\":\"Forss\",\"PaymentInstructions\":\"Skicka brevduva. Två gånger.\",\"DoneAddingExpenses\":true,\"HasPaid\":false},{\"Id\":30,\"Email\":\"harry@holkeri.fi\",\"Firstname\":\"Harry\",\"Lastname\":\"Holkeri\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true},{\"Id\":31,\"Email\":\"mama@mia.fi\",\"Firstname\":\"Mama\",\"Lastname\":\"Mia\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true}],\"Expenses\":[{\"Amount\":123.00000,\"Description\":\"adadad\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":1,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":432.00000,\"Description\":\"adssd\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31]},{\"Amount\":42.00000,\"Description\":\"dsaffds\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":212.00000,\"Description\":\"gfgfdg\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":31,\"UsersInDebtIds\":[30,31,1]}],\"Debts\":[{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30},{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30}]}";

        private long id;

        public ShowDebtCalculation()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            id = long.Parse(NavigationContext.QueryString["debtCalculationId"]);

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            var webClient = ApiHelper.AuthClient();
            webClient.DownloadStringCompleted += ShowCalculation;
            webClient.DownloadStringAsync(ApiHelper.GetFullApiCallUri("api/debtcalculation/show/" + id));

            //ShowCalculation(null, null);

        }

        private void ShowCalculation(object target, DownloadStringCompletedEventArgs eventArgs)
        {
            ContentPanel.Children.Clear();

            var calc = EntityCreator.DebtCalculationFromJson(eventArgs.Result);
            //var calc = EntityCreator.DebtCalculationFromJson(json);
            var currentUser = ApiHelper.CurrentUser(calc);

            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar
            {
                BackgroundColor = Color.FromArgb(255, 0x86, 0xC4, 0x40), //"#86C440"
                Opacity = 1,
                ForegroundColor = Colors.White
            };

            if (calc.Phase == DebtCalculationPhase.GatheringParticipants)
            {
                var editButton = new ApplicationBarIconButton
                    {
                        Text = AppResources.Edit,
                        IconUri = new Uri("/Assets/Icons/edit.png", UriKind.Relative)
                    };

                editButton.Click += Some_Click;
                ApplicationBar.Buttons.Add(editButton);
            }

            if (calc.Phase == DebtCalculationPhase.CollectingExpenses && !currentUser.DoneAddingExpenses)
            {
                var doneExpensesButton = new ApplicationBarIconButton
                    {
                        Text = AppResources.AddedExpenses,
                        IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                    };

                doneExpensesButton.Click += Some_Click;
                ApplicationBar.Buttons.Add(doneExpensesButton);
            }

            if (calc.Phase == DebtCalculationPhase.CollectingPayments && !currentUser.HasPaid)
            {
                var donePaymentsButton = new ApplicationBarIconButton
                {
                    Text = AppResources.HasPaid,
                    IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                };

                donePaymentsButton.Click += Some_Click;
                ApplicationBar.Buttons.Add(donePaymentsButton);
            }

            PageHeader.Text = calc.Name;
        }

        private void Some_Click(object sender, EventArgs eventArgs)
        {
            // sync
        }
    }
}