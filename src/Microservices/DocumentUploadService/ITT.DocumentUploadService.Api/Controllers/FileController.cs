using ITT.DocumentUploadService.Application.Interfaces;
using ITT.DocumentUploadService.Shared.Dtos.Requests;
using ITT.DocumentUploadService.Shared.Dtos.Responses;
using ITT.Shared.Shared.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ITT.DocumentUploadService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        public FileController(IServiceManager ServiceManager)
        {
            serviceManager = ServiceManager;    
        }

        [HttpPost("uploadfile")]
        public async Task<ActionResult<ApiResponse<FileUploadResponse>>> UploadFile(FileUploadRequest fileUploadRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                    return ApiResponse<FileUploadResponse>.Failure((int)HttpStatusCode.BadRequest, $"{ModelState}");
                }

                var uploadResponse = await serviceManager.FileService.Create(fileUploadRequest);
                if (!uploadResponse.Successful)
                {
                    return ApiResponse<FileUploadResponse>.Failure((int)HttpStatusCode.InternalServerError, uploadResponse.Message!);

                }

                return ApiResponse<FileUploadResponse>.Success(uploadResponse.Data!, 1, uploadResponse.Message!);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost("uploadfiles")]
        public async Task<ActionResult<ApiResponse<List<FileUploadResponse>>>> UploadFiles(List<FileUploadRequest> fileUploadRequests)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    return ApiResponse<List<FileUploadResponse>>.Failure((int)HttpStatusCode.BadRequest, $"{ModelState}");
                }

                var uploadResponse = await serviceManager.FileService.CreateBatch(fileUploadRequests);
                if (!uploadResponse.Successful)
                {
                    return ApiResponse<List<FileUploadResponse>>.Failure((int)HttpStatusCode.InternalServerError, uploadResponse.Message!);

                }

                return ApiResponse<List<FileUploadResponse>>.Success(uploadResponse.Data!, uploadResponse.Data!.Count, uploadResponse.Message!);

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
