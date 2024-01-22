using ITT.SummaryService.Application.Interfaces;
using ITT.SummaryService.Application.IRepositories;
using ITT.SummaryService.Persistence.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Infrastructure.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        public ITextSummaryRepository TextSummaryRepository { get; private set; }
        private readonly DBContext context;

        public UnitOfWork(ITextSummaryRepository TextSummaryRepository, DBContext Context)
        {
            this.TextSummaryRepository = TextSummaryRepository;
            context = Context;
        }


        public Task<int> CompleteAsync()
        {
            var result = context.SaveChangesAsync();
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }


    }
}
