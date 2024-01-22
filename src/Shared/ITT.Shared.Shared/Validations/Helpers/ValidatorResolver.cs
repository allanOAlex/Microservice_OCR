using FluentValidation;
using ITT.Shared.Shared.Validations.RequestValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Validations.Helpers
{
    public static class ValidatorResolver
    {
        public static IValidator ResolveValidator(Type requestType)
        {
            RequestValidator validator = new();
            return validator;

        }

    }

}
