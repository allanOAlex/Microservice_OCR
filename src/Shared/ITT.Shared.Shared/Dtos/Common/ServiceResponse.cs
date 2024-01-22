using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Dtos.Common
{
    public class ServiceResponse<T>
    {
        public int Id { get; set; }
        public bool Successful { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }


    }
}
