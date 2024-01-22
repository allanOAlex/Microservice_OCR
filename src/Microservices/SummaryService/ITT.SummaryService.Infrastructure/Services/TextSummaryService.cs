using AutoMapper;
using ITT.Shared.Shared.Dtos.Common;
using ITT.Shared.Shared.Exceptions;
using ITT.SummaryService.Application.Interfaces;
using ITT.SummaryService.Application.IServices;
using ITT.SummaryService.Domain.Entities;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Dtos.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Infrastructure.Services
{
    internal sealed class TextSummaryService : ITextSummaryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        public TextSummaryService(IUnitOfWork UnitOfWork, IMapper Mapper, IHttpClientFactory HttpClientFactory, IConfiguration Configuration)
        {
            unitOfWork = UnitOfWork;
            mapper = Mapper;
            httpClientFactory = HttpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            configuration = Configuration ?? throw new ArgumentNullException(nameof(configuration));
            
        }

        public async Task<ServiceResponse<CreateTextSummaryResponse>> Create(CreateTextSummaryRequest createTextSummaryRequest)
        {
            try
            {
                IEnumerable<TextSummary> textSummary = await unitOfWork.TextSummaryRepository.FindAll();
                var exists = textSummary.AsQueryable().Where(row =>
                row.DocumentId.Equals(createTextSummaryRequest.DocumentId) &&
                row.DcoumentTypeId.Equals(createTextSummaryRequest.DcoumentTypeId) &&
                EF.Functions.Like(row.DocumentName!, createTextSummaryRequest.DocumentName) &&
                EF.Functions.Like(row.TextContent, createTextSummaryRequest.TextContent)
                

                );

                if (exists.Any())
                    throw new CreatingDuplicateException("Duplicate Data");

                //ToDo Performa the Summarization here
                //Assign the summary value to [createTextSummaryRequest]

                var request = new MapperConfiguration(cfg => cfg.CreateMap<CreateTextSummaryRequest, TextSummary>());
                var response = new MapperConfiguration(cfg => cfg.CreateMap<TextSummary, CreateTextSummaryResponse>());

                IMapper requestMap = request.CreateMapper();
                IMapper responseMap = response.CreateMapper();

                var destination = requestMap.Map<CreateTextSummaryRequest, TextSummary>(createTextSummaryRequest);
                var itemToCreate = await unitOfWork.TextSummaryRepository.Create(destination);

                return await unitOfWork.CompleteAsync() >= 1
                    ? new ServiceResponse<CreateTextSummaryResponse>
                    {
                        Successful = true,
                        Message = "Summary created successfully!",
                        Data = new CreateTextSummaryResponse
                        {
                            DocumentName = itemToCreate.DocumentName,
                            TextContent = itemToCreate.TextContent,
                            TextSummary = itemToCreate.Summary,
                            
                        }
                    }
                    : new ServiceResponse<CreateTextSummaryResponse>
                    {
                        Successful = false,
                        Message = "Failed to create summary",
                        Data = new CreateTextSummaryResponse()
                    };

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<List<CreateTextSummaryResponse>>> CreateBatch(List<CreateTextSummaryRequest> createTextSummaryRequests)
        {
            try
            {
                List<CreateTextSummaryResponse> summaries = new();
                foreach (var createTextSummaryRequest in createTextSummaryRequests)
                {
                    IEnumerable<TextSummary> textSummary = await unitOfWork.TextSummaryRepository.FindAll();
                    var exists = textSummary.AsQueryable().Where(row =>
                    row.DocumentId.Equals(createTextSummaryRequest.DocumentId) &&
                    row.DcoumentTypeId.Equals(createTextSummaryRequest.DcoumentTypeId) &&
                    EF.Functions.Like(row.DocumentName!, createTextSummaryRequest.DocumentName) &&
                    EF.Functions.Like(row.TextContent, createTextSummaryRequest.TextContent)


                    );

                    if (exists.Any())
                        throw new CreatingDuplicateException("Duplicate Data");

                    var request = new MapperConfiguration(cfg => cfg.CreateMap<CreateTextSummaryRequest, TextSummary>());
                    var response = new MapperConfiguration(cfg => cfg.CreateMap<TextSummary, CreateTextSummaryResponse>());

                    IMapper requestMap = request.CreateMapper();
                    IMapper responseMap = response.CreateMapper();

                    var destination = requestMap.Map<CreateTextSummaryRequest, TextSummary>(createTextSummaryRequest);
                    var itemToCreate = await unitOfWork.TextSummaryRepository.Create(destination);

                    var summary = new CreateTextSummaryResponse
                    {
                        DocumentName = itemToCreate.DocumentName,
                        TextContent = itemToCreate.TextContent,
                        TextSummary = itemToCreate.Summary
                    };

                    summaries.Add(summary);

                }

                return await unitOfWork.CompleteAsync() >= 1
                    ? new ServiceResponse<List<CreateTextSummaryResponse>>
                    {
                        Successful = true,
                        Message = "Summary generated successfully!",
                        Data = summaries
                    }
                    : new ServiceResponse<List<CreateTextSummaryResponse>>
                    {
                        Successful = false,
                        Message = "Failed to generate summary",
                        Data = summaries
                    };


            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<TextSummaryResponse>>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<TextSummaryResponse>> FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<TextSummaryResponse>>> SummarizeBatch(List<TextSummaryRequest> textSummaryRequests)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<TextSummaryResponse>> SummarizeText(TextSummaryRequest textSummaryRequest)
        {
            try
            {
                string textSummary = string.Empty;

                using (var client = httpClientFactory.CreateClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(configuration["NLP:Url"]!),
                        Headers =
                        {
                            { "accept", "application/json"
                            },
                            { "authorization", $"Bearer {configuration["NLP:AuthKey"]}"
                            },
                        },
                        Content = new StringContent(GetRequestBody(textSummaryRequest))
                        {
                            Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                        }
                    };


                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        textSummary = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(textSummary);
                    }

                    return new ServiceResponse<TextSummaryResponse>
                    {
                        Successful = true,
                        Message = "Summary created successfully!",
                        Data = new TextSummaryResponse
                        {
                            DocumentName = textSummaryRequest.DocumentName,
                            TextContent = textSummaryRequest.TextContent,
                            TextSummary = textSummary,

                        }
                    };

                    string GetRequestBody(TextSummaryRequest textSummaryRequest)
                    {
                        var requestBody = new
                        {
                            response_as_dict = true,
                            attributes_as_list = false,
                            show_original_response = false,
                            output_sentences = 1,
                            providers = "nlpcloud",
                            language = "en",
                            text = textSummaryRequest.TextContent
                        };

                        return JsonConvert.SerializeObject(requestBody);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        


    }
}
