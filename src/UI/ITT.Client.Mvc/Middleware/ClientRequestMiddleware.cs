using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace ITT.Client.Mvc.Middleware
{
    public class ClientRequestMiddleware
    {
        private readonly RequestDelegate next;

        public ClientRequestMiddleware(RequestDelegate Next)
        {
            next = Next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);

            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                Log.Information("Exception: {Method} {Path} {Time} {RequestBody} {ExceptionType} {NewLine}", context.Request.Method, context.Request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")), exception.GetType(), "\n");

                Log.Information("Exception: {Method} {Path} {Time} {RequestBody} {ExceptionType} {NewLine}", context.Request.Method, context.Request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")), exception.GetType(), "\n");

                var request = context.Request;
                var response = context.Response;
                response.Clear();

                int statusCode;
                string errorId = Guid.NewGuid().ToString();
                var exceptionType = exception.GetType();
                string? message = string.Empty;
                var stackTrace = string.Empty;

                switch (exceptionType.Name)
                {
                    case nameof(HttpRequestException):
                        statusCode = ExtractStatusCodeFromMessage(exception.Message);
                        message = $"{exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(ValidationException):
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = $"{exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(NotImplementedException):
                        statusCode = (int)HttpStatusCode.NotImplemented;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(UnauthorizedAccessException):
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        message = exception.Message;
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(DbUpdateConcurrencyException):
                        statusCode = (int)HttpStatusCode.Conflict;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(NullReferenceException):
                        statusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(InvalidOperationException):
                        statusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(InvalidCastException):
                        statusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(IOException):
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        message = exception.Message;
                        stackTrace = exception.StackTrace;
                        break;
                }

                response.StatusCode = statusCode;

                if (!response.HasStarted)
                {
                    ProblemDetails problemDetails = new ProblemDetails()
                    {
                        Type = exceptionType.Name,
                        Title = exceptionType.Name,
                        Status = response.StatusCode,
                        Detail = message,
                        Instance = request.Path,

                    };

                    response.ContentType = "application/json";

                    //await response.WriteAsync(JsonConvert.SerializeObject(ApiResponse<ProblemDetails>.Failure(problemDetails)));
                    response.Redirect($"/Error/HandleError/{statusCode}");
                    return;

                }
                else
                {
                    Log.Warning("Can't write error response. Response has already started.");
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error handling exception: {ExceptionType} - {Message}", ex.GetType(), ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
            }


            int ExtractStatusCodeFromMessage(string message)
            {
                // Example regular expression to extract the status code from the message
                var match = Regex.Match(message, @"(\d{3})");

                if (match.Success && int.TryParse(match.Groups[1].Value, out int statusCode))
                {
                    return statusCode;
                }

                // Default to InternalServerError if status code extraction fails
                return (int)HttpStatusCode.InternalServerError;
            }

        }





    }

}
