using ITT.Shared.Shared.Dtos.Common;
using ITT.SummaryService.Application.Interfaces;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ITT.SummaryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextSummaryController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public TextSummaryController(IServiceManager ServiceManager)
        {
            serviceManager = ServiceManager;  
        }

        [HttpPost("summarizetext")]
        public async Task<ActionResult<ApiResponse<TextSummaryResponse>>> SummarizeText(TextSummaryRequest textSummaryRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse<TextSummaryResponse>.Failure((int)HttpStatusCode.BadRequest, $"{ModelState}");
                }

                var response = await serviceManager.TextSummaryService.SummarizeText(textSummaryRequest);
                if (!response.Successful)
                {
                    return ApiResponse<TextSummaryResponse>.Failure((int)HttpStatusCode.InternalServerError, response.Message!);

                }

                return ApiResponse<TextSummaryResponse>.Success(response.Data!, 1, response.Message!);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("summarizebatch")]
        public async Task<ActionResult<ApiResponse<List<TextSummaryResponse>>>> SummarizeBatch(List<TextSummaryRequest> textSummaryRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse<List<TextSummaryResponse>>.Failure((int)HttpStatusCode.BadRequest, $"{ModelState}");
                }

                var response = await serviceManager.TextSummaryService.SummarizeBatch(textSummaryRequest);
                if (!response.Successful)
                {
                    return ApiResponse<List<TextSummaryResponse>>.Failure((int)HttpStatusCode.InternalServerError, response.Message!);

                }

                return ApiResponse<List<TextSummaryResponse>>.Success(response.Data!, response.Data!.Count, response.Message!);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
