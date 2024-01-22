using AutoMapper;
using Azure;
using ITT.DocumentUploadService.Application.Interfaces;
using ITT.DocumentUploadService.Application.IServices;
using ITT.DocumentUploadService.Domain.Entities;
using ITT.DocumentUploadService.Shared.Dtos.Requests;
using ITT.DocumentUploadService.Shared.Dtos.Responses;
using ITT.Shared.Shared.Dtos.Common;
using ITT.Shared.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITT.DocumentUploadService.Infrastructure.Services
{
    internal sealed class FileService : IFileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public FileService(IUnitOfWork UnitOfWork, IMapper Mapper)
        {
            unitOfWork = UnitOfWork;
            mapper = Mapper;
        }

        public async Task<ServiceResponse<FileUploadResponse>> Create(FileUploadRequest fileUploadRequest)
        {
            try
            {
                IEnumerable<FileUpload> fileUpload = await unitOfWork.FileRepository.FindAll();
                var exists = fileUpload.AsQueryable().Where(row =>
                row.FileId.Equals(fileUploadRequest.FileId) &&
                row.FileType.Equals(fileUploadRequest.FileType) &&
                EF.Functions.Like(row.FileName!, fileUploadRequest.FileName) &&
                EF.Functions.Like(row.FileExtenstion, fileUploadRequest.FileExtension) &&
                row.FileData!.Equals(fileUploadRequest.FileData) &&
                row.DateCreated.Equals(fileUploadRequest.DateCreated)

                );

                if (exists.Any())
                    throw new CreatingDuplicateException("Duplicate Record");

                var request = new MapperConfiguration(cfg => cfg.CreateMap<FileUploadRequest, FileUpload>());
                var response = new MapperConfiguration(cfg => cfg.CreateMap<FileUpload, FileUploadResponse>());

                IMapper requestMap = request.CreateMapper();
                IMapper responseMap = response.CreateMapper();

                var destination = requestMap.Map<FileUploadRequest, FileUpload>(fileUploadRequest);
                var itemToCreate = await unitOfWork.FileRepository.Create(destination);

                return await unitOfWork.CompleteAsync() >= 1
                    ? new ServiceResponse<FileUploadResponse>
                    {
                        Successful = true,
                        Message = "File Created Successful",
                        Data = new FileUploadResponse
                        {
                            Id = itemToCreate.FileId,
                            FileType = itemToCreate.FileType,
                            FileName = itemToCreate.FileName,
                            FileExtension = itemToCreate.FileExtenstion
                        }
                    }
                    : new ServiceResponse<FileUploadResponse>
                    {
                        Successful = false,
                        Message = "File was not uploaded",
                        Data = new FileUploadResponse()
                    };

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<List<FileUploadResponse>>> CreateBatch(List<FileUploadRequest> fileUploadRequests)
        {
            try
            {
                List<FileUploadResponse> createdFiles = new();
                foreach (var fileUploadRequest in fileUploadRequests)
                {
                    IEnumerable<FileUpload> fileUpload = await unitOfWork.FileRepository.FindAll();
                    var exists = fileUpload.AsQueryable().Where(row =>
                    row.FileId.Equals(fileUploadRequest.FileId) &&
                    row.FileType.Equals(fileUploadRequest.FileType) &&
                    EF.Functions.Like(row.FileName!, fileUploadRequest.FileName) &&
                    EF.Functions.Like(row.FileExtenstion, fileUploadRequest.FileExtension) &&
                    row.FileData!.Equals(fileUploadRequest.FileData) &&
                    row.DateCreated.Equals(fileUploadRequest.DateCreated)

                    );

                    if (exists.Any())
                        throw new CreatingDuplicateException("Duplicate Record");


                    var request = new MapperConfiguration(cfg => cfg.CreateMap<FileUploadRequest, FileUpload>());
                    var response = new MapperConfiguration(cfg => cfg.CreateMap<FileUpload, FileUploadResponse>());

                    IMapper requestMap = request.CreateMapper();
                    IMapper responseMap = response.CreateMapper();

                    var destination = requestMap.Map<FileUploadRequest, FileUpload>(fileUploadRequest);
                    var itemToCreate = await unitOfWork.FileRepository.Create(destination);

                    var createdFile = new FileUploadResponse
                    {
                        Id = itemToCreate.FileId,
                        FileType = itemToCreate.FileType,
                        FileName = itemToCreate.FileName,
                        FileExtension = itemToCreate.FileExtenstion
                    };

                    createdFiles.Add(createdFile);
                }

                return await unitOfWork.CompleteAsync() >= 1
                    ? new ServiceResponse<List<FileUploadResponse>>
                    {
                        Successful = true,
                        Message = "User created successfully!",
                        Data = createdFiles
                    }
                    : new ServiceResponse<List<FileUploadResponse>>
                    {
                        Successful = false,
                        Message = "File was not uploaded",
                        Data = createdFiles
                    };


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ServiceResponse<List<FileUploadResponse>>> FindAll()
        {
            try
            {
                List<FileUploadResponse> fileUploadResponseList = new();
                var fileUploads = await unitOfWork.FileRepository.FindAll();
                if (fileUploads.Any())
                {
                    foreach (var item in fileUploads)
                    {
                        var listItem = new FileUploadResponse
                        {
                            Id = item.FileId,
                            FileType = item.FileType,
                            FileName = item.FileName,
                            FileExtension = item.FileExtenstion


                        };

                        fileUploadResponseList.Add(listItem);
                    }

                    return new ServiceResponse<List<FileUploadResponse>>
                    {
                        Successful = true,
                        Message = "",
                        Data = fileUploadResponseList
                    };

                }

                return new ServiceResponse<List<FileUploadResponse>>
                {
                    Successful = false,
                    Message = "",
                    Data = fileUploadResponseList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<FileUploadResponse>> FindById(int Id)
        {
            try
            {
                var itemToFind = await unitOfWork.FileRepository.FindByCondition(e => e.Id == Id);
                if (itemToFind.Any())
                {
                    var response = from e in itemToFind
                                   select new FileUploadResponse
                                   {
                                       Id = e.FileId,
                                       FileType = e.FileType,
                                       FileName = e.FileName,
                                       FileExtension = e.FileExtenstion
                                   };

                    return new ServiceResponse<FileUploadResponse>
                    {
                        Successful = true,
                        Message = "",
                        Data = response.FirstOrDefault()
                    };
                }

                return new ServiceResponse<FileUploadResponse>
                {
                    Successful = true,
                    Message = $"Document with Id - {Id} not found.",
                    Data = new FileUploadResponse()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<FileUploadRequest>> ProcessFile(IFormFile file)
        {
            try
            {
                var fileData = new FileUploadRequest
                {
                    FileId = 1,
                    FileType = 1,
                    FileName = file.FileName,
                    FileExtension = Path.GetExtension(file.FileName)?.TrimStart('.'),
                    FileContentType = file.ContentType,
                    FileData = await ConvertFileToBytesAsync(file),
                    DateCreated = DateTimeOffset.Now

                };

                return new ServiceResponse<FileUploadRequest>
                {
                    Successful = true,
                    Message = "Success",
                    Data = fileData
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ServiceResponse<List<FileUploadRequest>>> ProcessFiles(List<IFormFile> files)
        {
            try
            {
                List<FileUploadRequest> processedFiles = new();
                foreach (var file in files)
                {
                    var fileData = new FileUploadRequest
                    {
                        FileId = 1,
                        FileType = 1,
                        FileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName)?.TrimStart('.'),
                        FileContentType = file.ContentType,
                        FileData = await ConvertFileToBytesAsync(file),
                        DateCreated = DateTimeOffset.Now

                    };

                    processedFiles.Add(fileData);
                }

                return processedFiles.Count >= 1 
                    ? new ServiceResponse<List<FileUploadRequest>>
                    {
                        Successful = true,
                        Message = "Success",
                        Data = processedFiles
                    }
                    : new ServiceResponse<List<FileUploadRequest>>
                    {
                        Successful = false,
                        Message = "Failed",
                        Data = processedFiles
                    };

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> ConvertToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }

        private async Task<byte[]> ConvertFileToBytesAsync(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        
    }
}
