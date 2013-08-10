using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sujut.Core
{
    public class EntityCreator
    {
        public static IEnumerable<DebtCalculation> DebtCalculationFromJson(string json)
        {
            return DebtCalculationFromJson(JArray.Parse(json));
        }

        public static IEnumerable<DebtCalculation> DebtCalculationFromJson(JArray array)
        {
            var list = new List<DebtCalculation>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtCalculationFromJson(entry));
            }

            return list;
        }

        public static DebtCalculation DebtCalculationFromJson(dynamic obj)
        {
            var calc = new DebtCalculation
            {
                Id = long.Parse(obj.Id),
                Name = obj.Name,
                Phase = obj.Phase,
                LastActivityTime = DateTime.Parse(obj.LastActivityTime)
            };

            if (obj.Currency != null) // Currency should never be null in "full" calculation
            {
                calc.Currency = obj.Currency;
                calc.CreatorId = long.Parse(obj.CreatorId);
                calc.Description = obj.Description;
                calc.Participants = ParticipantFromJson((JArray) obj.Participants);
                calc.Expenses = ExpenseFromJson((JArray) obj.Expenses);
                calc.Debts = DebtFromJson((JArray) obj.Debts);
            }

            return calc;
        }

        public static IEnumerable<Participant> ParticipantFromJson(string json)
        {
            return ParticipantFromJson(JArray.Parse(json));
        }

        public static IEnumerable<Participant> ParticipantFromJson(JArray array)
        {
            var list = new List<Participant>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ParticipantFromJson(entry));
            }

            return list;
        }

        public static Participant ParticipantFromJson(dynamic obj)
        {
            var participant = new Participant
            {
                Firstname = obj.Firstname,
                Lastname = obj.Lastname,
                Email = obj.Email,
                PaymentInstructions = obj.PaymentInstructions,
                Id = long.Parse(obj.Id),
                HasPaid = bool.Parse(obj.HasPaid),
                DoneAddingExpenses = bool.Parse(obj.DoneAddingExpenses)
            };

            return participant;
        }

        public static IEnumerable<Expense> ExpenseFromJson(string json)
        {
            return ExpenseFromJson(JArray.Parse(json));
        }

        public static IEnumerable<Expense> ExpenseFromJson(JArray array)
        {
            var list = new List<Expense>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ExpenseFromJson(entry));
            }

            return list;
        }

        public static Expense ExpenseFromJson(dynamic obj)
        {
            var expense = new Expense
                {
                    Description = obj.Description,
                    Amount = decimal.Parse(obj.Amount),
                    AddedTime = DateTime.Parse(obj.AddedTime),
                    PayerId = long.Parse(obj.PayerId),
                    UsersInDebtIds = ((IEnumerable<string>) obj.UsersInDebtIds).Select(long.Parse).ToList()
                };

            return expense;
        }

        public static IEnumerable<Debt> DebtFromJson(string json)
        {
            return DebtFromJson(JArray.Parse(json));
        }

        public static  IEnumerable<Debt> DebtFromJson(JArray array)
        {
            var list = new List<Debt>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtFromJson(entry));
            }

            return list;
        }

        public static Debt DebtFromJson(dynamic obj)
        {
            var debt = new Debt
                {
                    Amount = decimal.Parse(obj.Amount),
                    CreditorId = long.Parse(obj.CreditorId),
                    DebtorId = long.Parse(obj.DebtorId)
                };

            return debt;
        }
    }
}
