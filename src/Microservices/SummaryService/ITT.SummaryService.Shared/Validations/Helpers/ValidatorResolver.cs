using FluentValidation;
using ITT.Shared.Shared.Validations.RequestValidators;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Validations.ModelValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Shared.Validations.Helpers
{
    public static class ValidatorResolver
    {
        public static IValidator ResolveValidator(Type requestType)
        {
            if (requestType == typeof(TextSummaryRequest))
            {
                TextSummaryRequestValidator validator = new();
                return validator;
            }
            else
            {
                RequestValidator validator = new();
                return validator;
            }

        }

    }
}
