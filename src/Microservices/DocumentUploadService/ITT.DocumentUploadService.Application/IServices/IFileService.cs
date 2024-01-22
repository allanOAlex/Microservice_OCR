using ITT.DocumentUploadService.Shared.Dtos.Requests;
using ITT.DocumentUploadService.Shared.Dtos.Responses;
using ITT.Shared.Shared.Dtos.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Application.IServices
{
    public interface IFileService
    {
        Task<ServiceResponse<FileUploadResponse>> Create(FileUploadRequest fileUploadRequest);
        Task<ServiceResponse<List<FileUploadResponse>>> CreateBatch(List<FileUploadRequest> fileUploadRequests);
        Task<ServiceResponse<FileUploadResponse>> FindById(int Id);
        Task<ServiceResponse<List<FileUploadResponse>>> FindAll();
        Task<ServiceResponse<FileUploadRequest>> ProcessFile(IFormFile file);
        Task<ServiceResponse<List<FileUploadRequest>>> ProcessFiles(List<IFormFile> files);


    }
}
