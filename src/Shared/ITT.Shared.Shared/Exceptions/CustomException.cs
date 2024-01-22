using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Exceptions
{
    public class CustomException : Exception
    {
        public List<string>? errorMessages { get; }
        public HttpStatusCode statusCode { get; }

        public CustomException(string message, List<string>? ErrorMessages = default, HttpStatusCode StatusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            errorMessages = ErrorMessages;
            statusCode = StatusCode;
        }
    }


}
