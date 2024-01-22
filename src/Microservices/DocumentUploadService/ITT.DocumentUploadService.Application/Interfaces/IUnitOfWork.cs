using ITT.DocumentUploadService.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IFileRepository FileRepository { get; }


        Task<int> CompleteAsync();

    }
}
