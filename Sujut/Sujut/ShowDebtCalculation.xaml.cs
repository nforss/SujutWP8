using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

namespace Sujut
{
    public partial class ShowDebtCalculation : PhoneApplicationPage
    {
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
            
            // Cannot localize ApplicationBar unless it is created in code behind
            ApplicationBar = new ApplicationBar
            {
                BackgroundColor = Color.FromArgb(255, 0x86, 0xC4, 0x40), //"#86C440"
                Opacity = 1,
                ForegroundColor = Colors.White
            };

            ContentPanel.Children.Add(new ProgressBar { IsIndeterminate = true, Width = 300, Margin = new Thickness(0, 30, 0, 0) });

            var webClient = ApiHelper.AuthClient();
            webClient.DownloadStringCompleted += ShowCalculation;
            webClient.DownloadStringAsync(ApiHelper.GetFullApiCallUri("api/debtcalculation/show/" + id));
        }

        private void ShowCalculation(object target, DownloadStringCompletedEventArgs eventArgs)
        {
            var progBar = ContentPanel.Children.First(c => c is ProgressBar);
            ContentPanel.Children.Remove(progBar);

            var calc = EntityCreator.DebtCalculationFromJson(eventArgs.Result);
            var currentUser = ApiHelper.CurrentUser(calc);

            PageHeader.Text = calc.Name;
            Phase.Text = calc.Phase.ToString();
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

            if (calc.Phase == DebtCalculationPhase.CollectingExpenses)
            {
                MainPivot.Items.Add(ExpensesItem);

                ShowExpenses(calc);

                if (!currentUser.DoneAddingExpenses)
                {
                    var doneExpensesButton = new ApplicationBarIconButton
                        {
                            Text = AppResources.AddedExpenses,
                            IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                        };

                    doneExpensesButton.Click += Some_Click;
                    ApplicationBar.Buttons.Add(doneExpensesButton);
                }
            }

            if (calc.Phase == DebtCalculationPhase.CollectingPayments)
            {
                MainPivot.Items.Add(DebtsItem);
                MainPivot.Items.Add(ExpensesItem);

                ShowExpenses(calc);
                ShowDebts(calc);

                if (!currentUser.HasPaid)
                {
                    var donePaymentsButton = new ApplicationBarIconButton
                        {
                            Text = AppResources.HasPaid,
                            IconUri = new Uri("/Assets/Icons/check.png", UriKind.Relative)
                        };

                    donePaymentsButton.Click += Some_Click;
                    ApplicationBar.Buttons.Add(donePaymentsButton);
                }
            }

            MainPivot.Items.Add(ParticipantsItem);
        }

        private void ShowParticipants(DebtCalculation calc)
        {
            var i = 0;
            foreach (var participant in calc.Participants)
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ParticipantsAddedExpHeader.ActualWidth) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ParticipantsHasPaidHeader.ActualWidth) });
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                if (i % 2 == 0)
                {
                    grid.Background = (SolidColorBrush)Application.Current.Resources["SujutLightGreenBrush"];
                }

                var email = new TextBlock { Text = participant.Email };
                Grid.SetRow(email, 0);
                Grid.SetColumn(email, 0);
                grid.Children.Add(email);

                var addedExpenses = new TextBlock { Text = participant.DoneAddingExpenses ? "X" : " ", HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(addedExpenses, 0);
                Grid.SetColumn(addedExpenses, 1);
                grid.Children.Add(addedExpenses);

                var hasPaid = new TextBlock { Text = participant.HasPaid ? "X" : " ", HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(hasPaid, 0);
                Grid.SetColumn(hasPaid, 2);
                grid.Children.Add(hasPaid);

                if (participant.Firstname != null || participant.Lastname != null)
                {
                    var nameBlock = new TextBlock { Text = participant.Firstname + " " + participant.Lastname };
                    Grid.SetRow(nameBlock, 1);
                    Grid.SetColumn(nameBlock, 0);
                    grid.Children.Add(nameBlock);
                }

                ParticipantsList.Children.Add(grid);

                i++;
            }
        }

        private void ShowExpenses(DebtCalculation calc)
        {
            var totalParticipants = calc.Participants.Count();

            var i = 0;
            foreach (var expense in calc.Expenses)
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ExpensesAmountHeader.ActualWidth) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ExpensesParticipantsHeader.ActualWidth) });
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                if (i % 2 == 0)
                {
                    grid.Background = (SolidColorBrush)Application.Current.Resources["SujutLightGreenBrush"];
                }

                var payer = calc.Participants.Single(p => p.Id == expense.PayerId);
                var payerText = new TextBlock { Text = payer.Email };
                Grid.SetRow(payerText, 0);
                Grid.SetColumn(payerText, 0);
                grid.Children.Add(payerText);

                var amount = new TextBlock { Text = calc.AmountAsString(expense.Amount), HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(amount, 0);
                Grid.SetColumn(amount, 1);
                grid.Children.Add(amount);

                var participants = new TextBlock 
                { 
                    Text = expense.UsersInDebtIds.Count() + "/" + totalParticipants,
                    HorizontalAlignment = HorizontalAlignment.Center 
                };

                Grid.SetRow(participants, 0);
                Grid.SetColumn(participants, 2);
                grid.Children.Add(participants);

                var description = new TextBlock { Text = expense.Description };
                Grid.SetRow(description, 1);
                Grid.SetColumn(description, 0);
                grid.Children.Add(description);

                ExpensesList.Children.Add(grid);

                i++;
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

                var debtor = calc.Participants.Single(p => p.Id == debt.DebtorId);
                var debtorText = new TextBlock { Text = debtor.Email, HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(debtorText, 0);
                Grid.SetColumn(debtorText, 1);
                grid.Children.Add(debtorText);

                var creditorHeader = new TextBlock { Text = AppResources.To, FontWeight = FontWeights.Bold};
                Grid.SetRow(creditorHeader, 1);
                Grid.SetColumn(creditorHeader, 0);
                grid.Children.Add(creditorHeader);

                var creditor = calc.Participants.Single(p => p.Id == debt.CreditorId);
                var creditorText = new TextBlock { Text = creditor.Email, HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(creditorText, 1);
                Grid.SetColumn(creditorText, 1);
                grid.Children.Add(creditorText);

                var amountHeader = new TextBlock { Text = AppResources.Amount, FontWeight = FontWeights.Bold };
                Grid.SetRow(amountHeader, 2);
                Grid.SetColumn(amountHeader, 0);
                grid.Children.Add(amountHeader);

                var amount = new TextBlock { Text = calc.AmountAsString(debt.Amount), HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(amount, 2);
                Grid.SetColumn(amount, 1);
                grid.Children.Add(amount);

                var hasPaidHeader = new TextBlock { Text = AppResources.HasPaid, FontWeight = FontWeights.Bold };
                Grid.SetRow(hasPaidHeader, 3);
                Grid.SetColumn(hasPaidHeader, 0);
                grid.Children.Add(hasPaidHeader);

                var hasPaid = new TextBlock { Text = debtor.HasPaid ? "X" : " ", HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(hasPaid, 3);
                Grid.SetColumn(hasPaid, 1);
                grid.Children.Add(hasPaid);

                DebtsList.Children.Add(grid);

                i++;
            }
        }

        private void Some_Click(object sender, EventArgs eventArgs)
        {
            // sync
        }
    }
}