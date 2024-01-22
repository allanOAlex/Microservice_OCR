using FluentValidation;
using ITT.DocumentUploadService.Shared.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Shared.Validations.ModelValidators
{
    public class FileUploadRequestValidator : AbstractValidator<FileUploadRequest>
    {
        public FileUploadRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            When(x => x != null, () => {

                RuleFor(x => x.FileName)
                .NotEmpty();

            });

        }
    }
}
