using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class OldDebtCalculation
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public OldDebtCalculationPhase Phase { get; set; }

        public string Currency { get; set; }

        public DateTime? LastActivityTime { get; set; }

        public long CreatorId { get; set; }

        public IEnumerable<OldParticipant> Participants { get; set; }

        public IEnumerable<OldExpense> Expenses { get; set; }

        public IEnumerable<OldDebt> Debts { get; set; }
    }
}
