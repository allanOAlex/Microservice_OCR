using ITT.Client.Mvc.Dtos.Common;
using ITT.Client.Mvc.Dtos.Requests;
using ITT.Client.Mvc.Dtos.Responses;

namespace ITT.Client.Mvc.ApiClients
{
    public class TextSummaryApiClient : ITextSummaryApiClient
    {
        private readonly HttpClient client;
        private readonly IConfiguration config;
        public TextSummaryApiClient(IHttpClientFactory HttpClientFactory, IConfiguration Config)
        {
            client = HttpClientFactory.CreateClient("Client");
            config = Config;
        }

        public async Task<ApiResponse<TextSummaryResponse>> SummarizeText(TextSummaryRequest textSummaryRequest)
        {
            try
            {
                var url = $"{config!["Api:BaseUrl"]}{config["Api:Routes:SummaryService:SummarizeText"]}";
                var response = await client.PostAsJsonAsync(url, textSummaryRequest);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<TextSummaryResponse>.Failure((int)response.StatusCode, $"Upload Failed | {response.ReasonPhrase}");
                }

                HashSet<string> errors = new HashSet<string>();

                return ApiResponse<TextSummaryResponse>.Success((int)response.StatusCode, 1, $"Success | {response.ReasonPhrase}");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
