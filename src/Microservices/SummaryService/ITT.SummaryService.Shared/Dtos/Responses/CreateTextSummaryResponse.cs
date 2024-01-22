using ITT.Shared.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Shared.Dtos.Responses
{
    public record CreateTextSummaryResponse : Response
    {
        public string? DocumentName { get; init; }
        public string? TextContent { get; init; }
        public string? TextSummary { get; init; }
    }
}
