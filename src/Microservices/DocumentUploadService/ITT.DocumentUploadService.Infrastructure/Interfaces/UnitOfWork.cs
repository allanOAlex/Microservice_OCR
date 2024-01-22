using ITT.DocumentUploadService.Application.Interfaces;
using ITT.DocumentUploadService.Application.IRepositories;
using ITT.DocumentUploadService.Persistence.DataContext;

namespace ITT.DocumentUploadService.Infrastructure.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        public IFileRepository FileRepository { get; private set; }

        private readonly DBContext context;
        //private readonly DapperContext dapper;

        public UnitOfWork(IFileRepository FileRepository, DBContext Context)
        {
            this.FileRepository = FileRepository;
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
