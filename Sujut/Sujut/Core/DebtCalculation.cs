using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class DebtCalculation
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Phase { get; set; }

        public string Currency { get; set; }

        public DateTime LastActivityTime { get; set; }

        public long CreatorId { get; set; }

        public IEnumerable<Participant> Participants { get; set; }

        public IEnumerable<Expense> Expenses { get; set; }

        public IEnumerable<Debt> Debts { get; set; }
    }
}
