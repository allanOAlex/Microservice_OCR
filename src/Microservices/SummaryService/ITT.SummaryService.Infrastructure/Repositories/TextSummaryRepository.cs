using ITT.Shared.Application.IRepositories;
using ITT.SummaryService.Application.IRepositories;
using ITT.SummaryService.Domain.Entities;
using ITT.SummaryService.Persistence.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Infrastructure.Repositories
{
    internal sealed class TextSummaryRepository : IBaseRepository<TextSummary>, ITextSummaryRepository
    {
        private readonly DBContext context;

        public TextSummaryRepository(DBContext Context)
        {
            context = Context;  
        }

        public Task<TextSummary> Create(TextSummary entity)
        {
            throw new NotImplementedException();
        }

        public Task<TextSummary> Delete(TextSummary entity)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TextSummary>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TextSummary>> FindByCondition(Expression<Func<TextSummary, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<TextSummary?> FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<TextSummary> Update(TextSummary entity)
        {
            throw new NotImplementedException();
        }


    }
}
