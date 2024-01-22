using ITT.Client.Mvc.Dtos.Common;
using ITT.Client.Mvc.Dtos.Requests;
using ITT.Client.Mvc.Dtos.Responses;

namespace ITT.Client.Mvc.ApiClients
{
    public class DocumentApiClient : IDocumentApiClient
    {
        private readonly HttpClient client;
        private readonly IConfiguration config;

        public DocumentApiClient(IHttpClientFactory HttpClientFactory, IConfiguration Config)
        {
            client = HttpClientFactory.CreateClient("Client");
            config = Config;
        }

        public async Task<ApiResponse<FileUploadResponse>> UploadFile(FileUploadRequest fileUploadRequest)
        {
            try
            {
                var url = $"{config!["Api:BaseUrl"]}{config["Api:Routes:UploadService:UploadFile"]}";
                var response = await client.PostAsJsonAsync(url, fileUploadRequest);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<FileUploadResponse>.Failure((int)response.StatusCode, $"Upload Failed | {response.ReasonPhrase}");
                }

                HashSet<string> errors = new HashSet<string>();

                return ApiResponse<FileUploadResponse>.Success((int)response.StatusCode, 1, $"Success | {response.ReasonPhrase}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<List<FileUploadResponse>>> UploadFiles(List<FileUploadRequest> fileUploadRequests)
        {
            try
            {
                var url = $"{config!["Api:BaseUrl"]}{config["Api:Routes:UploadService:UploadFiles"]}";

                var response = await client.PostAsJsonAsync(url, fileUploadRequests);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<List<FileUploadResponse>>.Failure((int)response.StatusCode, $"Upload Failed | {response.ReasonPhrase}");
                }

                HashSet<string> errors = new HashSet<string>();

                return ApiResponse<List<FileUploadResponse>>.Success((int)response.StatusCode, 1, $"Success | {response.ReasonPhrase}");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
