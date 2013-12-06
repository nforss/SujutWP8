using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class OldDebt
    {
        public decimal Amount { get; set; }

        public long DebtorId { get; set; }
        public long CreditorId { get; set; }
    }
}
