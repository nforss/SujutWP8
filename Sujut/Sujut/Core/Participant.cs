using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sujut.Core
{
    public class OldParticipant
    {
        public virtual long Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Lastname { get; set; }

        public virtual string PaymentInstructions { get; set; }

        public virtual bool DoneAddingExpenses { get; set; }

        public virtual bool HasPaid { get; set; }
    }
}
