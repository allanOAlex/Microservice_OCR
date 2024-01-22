using FluentValidation;
using ITT.SummaryService.Shared.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Shared.Validations.ModelValidators
{
    public class TextSummaryRequestValidator : AbstractValidator<TextSummaryRequest>
    {
        public TextSummaryRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            When(x => x != null, () => {

                RuleFor(x => x.DocumentId)
                .NotEmpty();

                RuleFor(x => x.DcoumentTypeId)
                .NotEmpty();

                RuleFor(x => x.DocumentName)
                .NotEmpty();

                RuleFor(x => x.TextContent)
                .NotEmpty();

            });

        }
    }
}
