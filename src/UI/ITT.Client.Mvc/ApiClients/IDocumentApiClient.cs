using ITT.Client.Mvc.Dtos.Common;
using ITT.Client.Mvc.Dtos.Requests;
using ITT.Client.Mvc.Dtos.Responses;

namespace ITT.Client.Mvc.ApiClients
{
    public interface IDocumentApiClient
    {
        Task<ApiResponse<FileUploadResponse>> UploadFile(FileUploadRequest fileUploadRequest);
        Task<ApiResponse<List<FileUploadResponse>>> UploadFiles(List<FileUploadRequest> fileUploadRequests);

    }
}
