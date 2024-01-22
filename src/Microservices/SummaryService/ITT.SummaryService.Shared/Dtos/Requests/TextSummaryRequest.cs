using ITT.Shared.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Shared.Dtos.Requests
{
    public record TextSummaryRequest 
    {
        public int DocumentId { get; init; }
        public int DcoumentTypeId { get; init; }
        public string? DocumentName { get; init; }
        public string? TextContent { get; init; }
        public string? TextSummary { get; init; }


    }
}
