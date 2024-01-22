using ITT.SummaryService.Application.Interfaces;
using ITT.SummaryService.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Infrastructure.Interfaces
{
    public class ServiceManager : IServiceManager
    {
        public ITextSummaryService TextSummaryService { get; private set; }

        public ServiceManager(ITextSummaryService TextSummaryService)
        {
             this.TextSummaryService = TextSummaryService;
        }
    }
}
