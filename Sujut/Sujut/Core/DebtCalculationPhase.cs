using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public enum DebtCalculationPhase
    {
        Undefined,
        GatheringParticipants,
        CollectingExpenses,
        CollectingPayments,
        Finished,
        Canceled
    }
}
