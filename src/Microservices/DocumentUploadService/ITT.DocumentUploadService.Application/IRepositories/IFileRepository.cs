using ITT.DocumentUploadService.Domain.Entities;
using ITT.Shared.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Application.IRepositories
{
    public interface IFileRepository : IBaseRepository<FileUpload>
    {

    }
}
