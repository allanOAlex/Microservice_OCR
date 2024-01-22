using ITT.DocumentUploadService.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Application.Interfaces
{
    public interface IServiceManager
    {
        IFileService FileService { get; }

    }
}
