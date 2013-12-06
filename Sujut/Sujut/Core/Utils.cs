using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sujut.SujutApi;

namespace Sujut.Core
{
    public class Utils
    {
        public static string AmountAsString(DebtCalculation calc, decimal amount)
        {
            var numberFormat = (NumberFormatInfo)Thread.CurrentThread.CurrentCulture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = GetCurrencySymbol(calc);

            return amount.ToString("C", numberFormat);
        }

        // TODO: Find better way of doing this on WP8
        private static string GetCurrencySymbol(DebtCalculation calc)
        {
            switch (calc.Currency)
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
                    return calc.Currency;
            }
        }
    }
}
