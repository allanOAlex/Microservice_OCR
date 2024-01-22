using ITT.Client.Mvc.Dtos.Common;
using ITT.Client.Mvc.Dtos.Requests;
using ITT.Client.Mvc.Dtos.Responses;

namespace ITT.Client.Mvc.ApiClients
{
    
    public interface ITextSummaryApiClient
    {
        Task<ApiResponse<TextSummaryResponse>> SummarizeText(TextSummaryRequest textSummaryRequest);

    }
}
