using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sujut.Core
{
    public class EntityCreator
    {
        public const string JsonDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public static IEnumerable<DebtCalculation> DebtCalculationsFromJson(string json)
        {
            return DebtCalculationsFromJson(JArray.Parse(json));
        }

        public static IEnumerable<DebtCalculation> DebtCalculationsFromJson(JArray array)
        {
            var list = new List<DebtCalculation>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtCalculationFromJson(entry));
            }

            return list;
        }

        public static DebtCalculation DebtCalculationFromJson(string json)
        {
            return DebtCalculationFromJson(JObject.Parse(json));
        }

        public static DebtCalculation DebtCalculationFromJson(dynamic obj)
        {
            var calc = new DebtCalculation
            {
                Id = obj.Id,
                Name = obj.Name,
                Phase = (DebtCalculationPhase) obj.Phase,
                LastActivityTime = obj.LastActivityTime
            };

            if (obj.Currency != null) // Currency should never be null in "full" calculation
            {
                calc.Currency = obj.Currency;
                calc.CreatorId = obj.CreatorId;
                calc.Description = obj.Description;
                calc.Participants = ParticipantsFromJson((JArray) obj.Participants);
                calc.Expenses = ExpensesFromJson((JArray) obj.Expenses);
                calc.Debts = DebtsFromJson((JArray) obj.Debts);
            }

            return calc;
        }

        public static IEnumerable<Participant> ParticipantsFromJson(string json)
        {
            return ParticipantsFromJson(JArray.Parse(json));
        }

        public static IEnumerable<Participant> ParticipantsFromJson(JArray array)
        {
            var list = new List<Participant>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ParticipantFromJson(entry));
            }

            return list;
        }

        public static Participant ParticipantFromJson(string json)
        {
            return ParticipantFromJson(JObject.Parse(json));
        }

        public static Participant ParticipantFromJson(dynamic obj)
        {
            var participant = new Participant
            {
                Firstname = obj.Firstname,
                Lastname = obj.Lastname,
                Email = obj.Email,
                PaymentInstructions = obj.PaymentInstructions,
                Id = obj.Id,
                HasPaid = obj.HasPaid,
                DoneAddingExpenses = obj.DoneAddingExpenses
            };

            return participant;
        }

        public static IEnumerable<Expense> ExpensesFromJson(string json)
        {
            return ExpensesFromJson(JArray.Parse(json));
        }

        public static IEnumerable<Expense> ExpensesFromJson(JArray array)
        {
            var list = new List<Expense>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ExpenseFromJson(entry));
            }

            return list;
        }

        public static Expense ExpenseFromJson(string json)
        {
            return ExpenseFromJson(JObject.Parse(json));
        }

        public static Expense ExpenseFromJson(dynamic obj)
        {
            var expense = new Expense
                {
                    Description = obj.Description,
                    Amount = obj.Amount,
                    AddedTime = obj.AddedTime,
                    PayerId = obj.PayerId,
                    UsersInDebtIds = ((JArray)obj.UsersInDebtIds).Select(u => (long)u).ToList()
                };

            return expense;
        }

        public static IEnumerable<Debt> DebtsFromJson(string json)
        {
            return DebtsFromJson(JArray.Parse(json));
        }

        public static  IEnumerable<Debt> DebtsFromJson(JArray array)
        {
            var list = new List<Debt>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtFromJson(entry));
            }

            return list;
        }

        public static Debt DebtFromJson(string json)
        {
            return DebtFromJson(JObject.Parse(json));
        }

        public static Debt DebtFromJson(dynamic obj)
        {
            var debt = new Debt
                {
                    Amount = obj.Amount,
                    CreditorId = obj.CreditorId,
                    DebtorId = obj.DebtorId
                };

            return debt;
        }
    }
}
