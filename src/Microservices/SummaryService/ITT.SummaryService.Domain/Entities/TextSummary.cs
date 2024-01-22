using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Domain.Entities
{
    public class TextSummary
    {
        [Key]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int DcoumentTypeId { get; set; }
        public string? DocumentName { get; set; }
        public string? TextContent { get; set; }
        public string? Summary { get; set; }
        
    }
}
