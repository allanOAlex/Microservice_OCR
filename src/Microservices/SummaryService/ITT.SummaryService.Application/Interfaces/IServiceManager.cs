using ITT.SummaryService.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Application.Interfaces
{
    public interface IServiceManager
    {
        ITextSummaryService TextSummaryService { get; }
    }
}
