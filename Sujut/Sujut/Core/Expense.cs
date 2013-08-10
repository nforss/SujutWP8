using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class Expense
    {
        public virtual decimal Amount { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime AddedTime { get; set; }

        public long PayerId { get; set; }

        public IEnumerable<long> UsersInDebtIds { get; set; } 
    }
}
