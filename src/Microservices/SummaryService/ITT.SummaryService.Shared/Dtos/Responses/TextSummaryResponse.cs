using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITT.Shared.Shared.Dtos.Common;

namespace ITT.SummaryService.Shared.Dtos.Responses
{
    public record TextSummaryResponse : Response
    {
        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string? DocumentName { get; set; }
        public string? TextContent { get; set; }
        public string? TextSummary { get; set; }


    }
}
