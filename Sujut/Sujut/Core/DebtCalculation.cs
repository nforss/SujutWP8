using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class DebtCalculation
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DebtCalculationPhase Phase { get; set; }

        public string Currency { get; set; }

        public DateTime? LastActivityTime { get; set; }

        public long CreatorId { get; set; }

        public IEnumerable<Participant> Participants { get; set; }

        public IEnumerable<Expense> Expenses { get; set; }

        public IEnumerable<Debt> Debts { get; set; }

        public virtual string AmountAsString(decimal amount)
        {
            var numberFormat = (NumberFormatInfo)Thread.CurrentThread.CurrentCulture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = GetCurrencySymbol();

            return amount.ToString("C", numberFormat);
        }

        // TODO: Find better way of doing this on WP8
        private string GetCurrencySymbol()
        {
            switch (Currency)
            {
                case "EUR":
                    return "€";

                case "USD":
                    return "$";

                case "GPD":
                    return "£";

                case "SEK":
                    return "kr";

                default:
                    return Currency;
            }
        }
    }
}
