using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sujut.Api;
using Sujut.Core;
using Sujut.Resources;
using Sujut.SujutApi;
using Sujut.Helpers;
using DebtCalculation = Sujut.SujutApi.DebtCalculation;

namespace Sujut
{
    public partial class ShowDebtCalculation : PhoneApplicationPage
    {
        private Container _container;
        private DataServiceCollection<DebtCalculation> _calculations;

        private long id;

        PivotItem ParticipantsItem { get; set; }
        PivotItem ExpensesItem { get; set; }
        PivotItem DebtsItem { get; set; }

        public ShowDebtCalculation()
        {
            InitializeComponent();
            
            ParticipantsItem = Participants;
            ExpensesItem = Expenses;
            DebtsItem = Debts;

            MainPivot.Items.Remove(ParticipantsItem);
            MainPivot.Items.Remove(ExpensesItem);
            MainPivot.Items.Remove(DebtsItem);
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            id = long.Parse(NavigationContext.QueryString["debtCalculationId"]);

            _container = ApiHelper.GetContainer();

            _calculations = new DataServiceCollection<DebtCalculation>(_container);
            
            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar
            {
                BackgroundColor = Color.FromArgb(255, 0x86, 0xC4, 0x40), //"#86C440"
                Opacity = 1,
                ForegroundColor = Colors.White
            };

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            var query = _container.DebtCalculations
                .Expand("Expenses/Payer")
                .Expand("Expenses/Debtors/User")
                .Expand("Participants/User")
                .Expand("Debts/Debtor")
                .Expand("Debts/Creditor")
                .Expand(dc => dc.Creator)
                .Where(dc => dc.Id == id);

            _calculations.LoadCompleted += ShowCalculation;
            _calculations.LoadAsync(query);
        }

        private void ShowCalculation(object target, LoadCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            var calc = _calculations.SingleOrDefault();

            if (calc == null)
            {
                return;
            }

            var currentUserId = ApiHelper.CurrentUserId();

            PageHeader.Text = calc.Name;
            Phase.Text = calc.Phase;
            Description.Text = calc.Description;

            ShowParticipants(calc);

            //if (calc.Phase == DebtCalculationPhase.GatheringParticipants)
            //{
            //    var editButton = new ApplicationBarIconButton
            //        {
            //            Text = AppResources.Edit,
            //            IconUri = new Uri("/Assets/Icons/edit.png", UriKind.Relative)
            //        };

            //    editButton.Click += Some_Click;
            //    ApplicationBar.Buttons.Add(editButton);
            //}

            if (calc.Phase == DebtCalculationPhase.CollectingExpenses.ToString())
            {
                var addExpenseButton = new ApplicationBarIconButton
                    {
                        Text = AppResources.AddExpense,
                        IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative)
                    };

                addExpenseButton.Click += AddExpense_Click;
                ApplicationBar.Buttons.Add(addExpenseButton);

                MainPivot.Items.Add(ExpensesItem);

                ShowExpenses(calc);

                if (!calc.Participants.Single(p => p.User.Id == currentUserId).DoneAddingExpenses)
                {
                    var doneExpensesButton = new ApplicationBarIconButton
                        {
                            Text = AppResources.AddedExpenses,
                            IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                        };

                    doneExpensesButton.Click += AddedExpenses_Click;
                    ApplicationBar.Buttons.Add(doneExpensesButton);
                }
            }

            if (calc.Phase == DebtCalculationPhase.CollectingPayments.ToString() ||
                calc.Phase == DebtCalculationPhase.Finished.ToString())
            {
                MainPivot.Items.Add(DebtsItem);
                MainPivot.Items.Add(ExpensesItem);

                ShowExpenses(calc);
                ShowDebts(calc);

                if (!calc.Participants.Single(p => p.User.Id == currentUserId).HasPaid)
                {
                    var donePaymentsButton = new ApplicationBarIconButton
                        {
                            Text = AppResources.HasPaid,
                            IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                        };

                    donePaymentsButton.Click += HasPaid_Click;
                    ApplicationBar.Buttons.Add(donePaymentsButton);
                }
            }

            MainPivot.Items.Add(ParticipantsItem);
        }

        private void ShowParticipants(DebtCalculation calc)
        {
            var i = 1;
            foreach (var participant in calc.Participants)
            {
                ParticipantsGrid.RowDefinitions.Add(new RowDefinition());

                if (i % 2 == 1)
                {
                    var background = new Grid
                        {
                            Background = (SolidColorBrush) Application.Current.Resources["SujutLightGreenBrush"]
                        };

                    Grid.SetRow(background, i);
                    Grid.SetColumn(background, 0);
                    Grid.SetColumnSpan(background, 3);

                    ParticipantsGrid.Children.Add(background);
                }

                var name = new TextBlock { Text = participant.User.FullName() };
                Grid.SetRow(name, i);
                Grid.SetColumn(name, 0);
                ParticipantsGrid.Children.Add(name);

                var addedExpenses = new TextBlock
                    {
                        Text = participant.DoneAddingExpenses ? "✓" : " ",
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                Grid.SetRow(addedExpenses, i);
                Grid.SetColumn(addedExpenses, 1);
                ParticipantsGrid.Children.Add(addedExpenses);

                var hasPaid = new TextBlock
                    {
                        Text = participant.HasPaid ? "✓" : " ",
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                Grid.SetRow(hasPaid, i);
                Grid.SetColumn(hasPaid, 2);
                ParticipantsGrid.Children.Add(hasPaid);
                
                i++;
            }
        }

        private void ShowExpenses(DebtCalculation calc)
        {
            var totalParticipants = calc.Participants.Count();

            var i = 1;
            foreach (var expense in calc.Expenses)
            {
                ExpensesGrid.RowDefinitions.Add(new RowDefinition());
                ExpensesGrid.RowDefinitions.Add(new RowDefinition());

                if ((i/2) % 2 == 0)
                {
                    var background = new Grid
                    {
                        Background = (SolidColorBrush)Application.Current.Resources["SujutLightGreenBrush"]
                    };

                    Grid.SetRow(background, i);
                    Grid.SetColumn(background, 0);
                    Grid.SetColumnSpan(background, 3);

                    ExpensesGrid.Children.Add(background);

                    var background2 = new Grid
                    {
                        Background = (SolidColorBrush)Application.Current.Resources["SujutLightGreenBrush"]
                    };

                    Grid.SetRow(background2, i + 1);
                    Grid.SetColumn(background2, 0);
                    Grid.SetColumnSpan(background2, 3);

                    ExpensesGrid.Children.Add(background2);
                }

                var payer = calc.Participants.Single(p => p.User.Id == expense.Payer.Id);
                var payerText = new TextBlock { Text = payer.User.FullName() };
                Grid.SetRow(payerText, i);
                Grid.SetColumn(payerText, 0);
                ExpensesGrid.Children.Add(payerText);

                var amount = new TextBlock
                    {
                        Text = Utils.AmountAsString(calc, expense.Amount), 
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                Grid.SetRow(amount, i);
                Grid.SetColumn(amount, 1);
                ExpensesGrid.Children.Add(amount);

                var participants = new TextBlock 
                { 
                    Text = expense.Debtors.Count() + "/" + totalParticipants,
                    HorizontalAlignment = HorizontalAlignment.Center 
                };

                Grid.SetRow(participants, i);
                Grid.SetColumn(participants, 2);
                ExpensesGrid.Children.Add(participants);

                var description = new TextBlock { Text = expense.Description, TextWrapping = new TextWrapping()};
                Grid.SetRow(description, i+1);
                Grid.SetColumn(description, 0);
                Grid.SetColumnSpan(description, 3);
                ExpensesGrid.Children.Add(description);
                
                i += 2;
            }
        }

        private void ShowDebts(DebtCalculation calc)
        {
            var i = 0;
            foreach (var debt in calc.Debts)
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                if (i % 2 == 0)
                {
                    grid.Background = (SolidColorBrush)Application.Current.Resources["SujutLightGreenBrush"];
                }

                var debtorHeader = new TextBlock { Text = AppResources.From, FontWeight = FontWeights.Bold };
                Grid.SetRow(debtorHeader, 0);
                Grid.SetColumn(debtorHeader, 0);
                grid.Children.Add(debtorHeader);

                var debtor = calc.Participants.Single(p => p.User.Id == debt.Debtor.Id);
                var debtorText = new TextBlock { Text = debtor.User.FullName(), HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(debtorText, 0);
                Grid.SetColumn(debtorText, 1);
                grid.Children.Add(debtorText);

                var creditorHeader = new TextBlock { Text = AppResources.To, FontWeight = FontWeights.Bold};
                Grid.SetRow(creditorHeader, 1);
                Grid.SetColumn(creditorHeader, 0);
                grid.Children.Add(creditorHeader);

                var creditor = calc.Participants.Single(p => p.User.Id == debt.Creditor.Id);
                var creditorText = new TextBlock { Text = creditor.User.FullName(), HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(creditorText, 1);
                Grid.SetColumn(creditorText, 1);
                grid.Children.Add(creditorText);

                var amountHeader = new TextBlock { Text = AppResources.Amount, FontWeight = FontWeights.Bold };
                Grid.SetRow(amountHeader, 2);
                Grid.SetColumn(amountHeader, 0);
                grid.Children.Add(amountHeader);

                var amount = new TextBlock { Text = Utils.AmountAsString(calc, debt.Amount), HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(amount, 2);
                Grid.SetColumn(amount, 1);
                grid.Children.Add(amount);

                var hasPaidHeader = new TextBlock { Text = AppResources.HasPaid, FontWeight = FontWeights.Bold };
                Grid.SetRow(hasPaidHeader, 3);
                Grid.SetColumn(hasPaidHeader, 0);
                grid.Children.Add(hasPaidHeader);

                var hasPaid = new TextBlock { Text = debtor.HasPaid ? "✓" : " ", HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(hasPaid, 3);
                Grid.SetColumn(hasPaid, 1);
                grid.Children.Add(hasPaid);

                DebtsList.Children.Add(grid);

                i++;
            }
        }

        private void AddExpense_Click(object sender, EventArgs eventArgs)
        {
            NavigationService.Navigate(new Uri("/AddExpense.xaml?debtCalculationId=" + id +
                "&" + DateTime.Now.Ticks, UriKind.Relative));
        }

        private void AddedExpenses_Click(object sender, EventArgs eventArgs)
        {
            var webClient = ApiHelper.AuthClient();
            webClient.UploadStringCompleted += ReloadPage;
            webClient.UploadStringAsync(
                ApiHelper.GetFullApiCallUri("api/DebtCalculation("+ id + ")/DoneAddingExpenses"), "");
        }

        private void HasPaid_Click(object sender, EventArgs eventArgs)
        {
            var webClient = ApiHelper.AuthClient();
            webClient.UploadStringCompleted += ReloadPage;
            webClient.UploadStringAsync(ApiHelper.GetFullApiCallUri("api/DebtCalculation(" + id + ")/Paid"), "");
        }

        private void ReloadPage(object target, UploadStringCompletedEventArgs args)
        {
            NavigationService.Navigate(new Uri("/ShowDebtCalculation.xaml?debtCalculationId=" + id + 
                "&" + DateTime.Now.Ticks, UriKind.Relative));
        }
    }
}