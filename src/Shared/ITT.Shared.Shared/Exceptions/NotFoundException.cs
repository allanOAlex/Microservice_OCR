using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message, null, HttpStatusCode.NotFound)
        {
        }
    }
}
