using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Exceptions
{
    public class CreatingDuplicateException : Exception
    {
        public CreatingDuplicateException(string message = null!) : base(message: message)
        {
        }
    }
}
