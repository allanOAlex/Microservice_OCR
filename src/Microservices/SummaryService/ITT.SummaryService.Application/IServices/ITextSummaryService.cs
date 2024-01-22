using ITT.Shared.Shared.Dtos.Common;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Application.IServices
{
    public interface ITextSummaryService
    {
        Task<ServiceResponse<CreateTextSummaryResponse>> Create(CreateTextSummaryRequest fileUploadRequest);
        Task<ServiceResponse<List<CreateTextSummaryResponse>>> CreateBatch(List<CreateTextSummaryRequest> fileUploadRequests);
        Task<ServiceResponse<TextSummaryResponse>> FindById(int Id);
        Task<ServiceResponse<List<TextSummaryResponse>>> FindAll();
        Task<ServiceResponse<TextSummaryResponse>> SummarizeText(TextSummaryRequest textSummaryRequest);
        Task<ServiceResponse<List<TextSummaryResponse>>> SummarizeBatch(List<TextSummaryRequest> textSummaryRequests);

    }
}
