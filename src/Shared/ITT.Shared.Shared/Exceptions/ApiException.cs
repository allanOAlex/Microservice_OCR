using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Exceptions
{
    public class ApiException : HttpRequestException
    {
        public ApiException(ProblemDetails problemDetails) : base(problemDetails.Detail)
        {
            OriginalExceptionType = problemDetails.Type;
            OriginalExceptionTitle = problemDetails.Title;
            OriginalStatusCode = problemDetails.Status;
            OriginalExceptionMessage = problemDetails.Detail;
            OriginalPath = problemDetails.Instance;
            OriginalExceptionStackTrace = string.Empty;

        }

        public string OriginalExceptionType { get; }
        public string OriginalExceptionTitle { get; }
        public string OriginalExceptionMessage { get; }
        public string? OriginalExceptionStackTrace { get; }
        public int? OriginalStatusCode { get; }
        public string OriginalPath { get; }
    }

}
