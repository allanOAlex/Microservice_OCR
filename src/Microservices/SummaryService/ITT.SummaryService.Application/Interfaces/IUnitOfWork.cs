using ITT.SummaryService.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        ITextSummaryRepository TextSummaryRepository { get; }


        Task<int> CompleteAsync();
    }
}
