using ITT.Shared.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Shared.Dtos.Responses
{
    public record FileUploadResponse : Response
    {
        public int FileType { get; init; }
        public string? FileName { get; init; }
        public string? FileExtension { get; init; }


    }
}
