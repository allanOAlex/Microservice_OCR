using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Dtos.Common
{
    public record Response
    {
        public int Id { get; init; }
        public string? Description { get; init; }



    }
}
