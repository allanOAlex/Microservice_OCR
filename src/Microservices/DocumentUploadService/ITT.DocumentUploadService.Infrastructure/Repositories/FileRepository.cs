using ITT.DocumentUploadService.Application.IRepositories;
using ITT.DocumentUploadService.Domain.Entities;
using ITT.DocumentUploadService.Persistence.DataContext;
using ITT.Shared.Application.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Infrastructure.Repositories
{
    internal sealed class FileRepository : IBaseRepository<FileUpload>, IFileRepository
    {
        private readonly DBContext context;

        public FileRepository(DBContext Context)
        {
            
            context = Context;
        }

        public async Task<FileUpload> Create(FileUpload entity)
        {
            try
            {
                await context.FileUploads!.AddAsync(entity);
                return entity;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<FileUpload> Delete(FileUpload entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<FileUpload>> FindAll()
        {
            return await Task.FromResult(context.FileUploads!.OrderByDescending(e => e.Id).AsNoTracking());
        }

        public async Task<IQueryable<FileUpload>> FindByCondition(Expression<Func<FileUpload, bool>> expression)
        {
            
            return await Task.FromResult(context.FileUploads!.Where(expression).AsNoTracking());
        }

        public async Task<FileUpload?> FindById(int Id)
        {
            return await context.FileUploads!.FindAsync(Id);
        }

        public Task<FileUpload> Update(FileUpload entity)
        {
            throw new NotImplementedException();
        }



    }
}
