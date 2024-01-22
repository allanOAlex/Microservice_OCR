using ITT.DocumentUploadService.Application.Interfaces;
using ITT.DocumentUploadService.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Infrastructure.Interfaces
{
    public class ServiceManager : IServiceManager
    {
        public IFileService FileService { get; private set; }


        public ServiceManager(IFileService FileService)
        {
               this.FileService = FileService;
        }


    }
}
