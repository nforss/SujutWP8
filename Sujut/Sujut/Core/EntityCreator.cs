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

        public static IEnumerable<OldDebtCalculation> DebtCalculationsFromJson(string json)
        {
            return DebtCalculationsFromJson(JArray.Parse(json));
        }

        public static IEnumerable<OldDebtCalculation> DebtCalculationsFromJson(JArray array)
        {
            var list = new List<OldDebtCalculation>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtCalculationFromJson(entry));
            }

            return list;
        }

        public static OldDebtCalculation DebtCalculationFromJson(string json)
        {
            return DebtCalculationFromJson(JObject.Parse(json));
        }

        public static OldDebtCalculation DebtCalculationFromJson(dynamic obj)
        {
            var calc = new OldDebtCalculation
            {
                Id = obj.Id,
                Name = obj.Name,
                Phase = (OldDebtCalculationPhase) obj.Phase,
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

        public static IEnumerable<OldParticipant> ParticipantsFromJson(string json)
        {
            return ParticipantsFromJson(JArray.Parse(json));
        }

        public static IEnumerable<OldParticipant> ParticipantsFromJson(JArray array)
        {
            var list = new List<OldParticipant>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ParticipantFromJson(entry));
            }

            return list;
        }

        public static OldParticipant ParticipantFromJson(string json)
        {
            return ParticipantFromJson(JObject.Parse(json));
        }

        public static OldParticipant ParticipantFromJson(dynamic obj)
        {
            var participant = new OldParticipant
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

        public static IEnumerable<OldExpense> ExpensesFromJson(string json)
        {
            return ExpensesFromJson(JArray.Parse(json));
        }

        public static IEnumerable<OldExpense> ExpensesFromJson(JArray array)
        {
            var list = new List<OldExpense>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(ExpenseFromJson(entry));
            }

            return list;
        }

        public static OldExpense ExpenseFromJson(string json)
        {
            return ExpenseFromJson(JObject.Parse(json));
        }

        public static OldExpense ExpenseFromJson(dynamic obj)
        {
            var expense = new OldExpense
                {
                    Description = obj.Description,
                    Amount = obj.Amount,
                    AddedTime = obj.AddedTime,
                    PayerId = obj.PayerId,
                    UsersInDebtIds = ((JArray)obj.UsersInDebtIds).Select(u => (long)u).ToList()
                };

            return expense;
        }

        public static IEnumerable<OldDebt> DebtsFromJson(string json)
        {
            return DebtsFromJson(JArray.Parse(json));
        }

        public static  IEnumerable<OldDebt> DebtsFromJson(JArray array)
        {
            var list = new List<OldDebt>();

            foreach (dynamic entry in array.Children<JObject>())
            {
                list.Add(DebtFromJson(entry));
            }

            return list;
        }

        public static OldDebt DebtFromJson(string json)
        {
            return DebtFromJson(JObject.Parse(json));
        }

        public static OldDebt DebtFromJson(dynamic obj)
        {
            var debt = new OldDebt
                {
                    Amount = obj.Amount,
                    CreditorId = obj.CreditorId,
                    DebtorId = obj.DebtorId
                };

            return debt;
        }
    }
}
