using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sujut.SujutApi;

namespace Sujut.Helpers
{
    public static class EntityExtensions
    {
        public static string FullName(this User user)
        {
            return (user.Firstname + " " + user.Lastname).Trim();
        }
    }
}
