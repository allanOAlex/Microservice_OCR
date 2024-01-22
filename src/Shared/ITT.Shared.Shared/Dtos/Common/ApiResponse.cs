using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Dtos.Common
{
    public record ApiResponse<T> where T : class
    {
        public bool Successful { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public List<T>? Datas { get; init; }
        public int TotalRecords { get; init; }
        public int? StatusCode { get; init; }
        public string? Error { get; init; }
        public string? Instance { get; init; }
        public string? Type { get; init; }
        public string? Title { get; init; }



        public static ApiResponse<T> Success(T data, int totalRecords, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Successful = true,
                Message = message,
                Data = data,
                Datas = null,
                TotalRecords = totalRecords,
                Error = null
            };
        }

        public static ApiResponse<T> Success(List<T> datas, int totalRecords, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Successful = true,
                Message = message,
                Data = null,
                Datas = datas,
                TotalRecords = totalRecords,
                Error = null
            };
        }

        public static ApiResponse<T> Failure(int statusCode, string errorMessage)
        {
            return new ApiResponse<T>
            {
                Successful = false,
                Message = errorMessage,
                Data = null,
                Datas = null,
                StatusCode = statusCode,
                Error = errorMessage
            };
        }

        public static ApiResponse<T> Failure(ProblemDetails problemDetails)
        {
            return new ApiResponse<T>
            {
                Successful = false,
                Message = problemDetails.Detail,
                Type = problemDetails.Type,
                Title = problemDetails.Title,
                StatusCode = problemDetails.Status,
                Instance = problemDetails.Instance
            };
        }


    }

}
