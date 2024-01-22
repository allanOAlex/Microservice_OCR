using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Dtos.Common
{
    public record ApiRequest<T> where T : class
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int Skip { get; init; }
        public T? Request { get; init; }

    }
}
