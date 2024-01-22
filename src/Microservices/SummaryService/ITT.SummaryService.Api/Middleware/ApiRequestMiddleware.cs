using FluentValidation;
using ITT.Shared.Shared.Dtos.Common;
using ITT.Shared.Shared.Exceptions;
using ITT.SummaryService.Api.Helpers.MiddlewareHelpers;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Validations.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace ITT.SummaryService.Api.Middleware
{
    public class ApiRequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApiRequestMiddleware(RequestDelegate Next, IHttpContextAccessor HttpContextAccessor)
        {
            next = Next;
            httpContextAccessor = HttpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var path = request.Path;

            try
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    Log.Information("Incoming Request: {Method} {Path} {Time} {RequestBody} {NewLine}", context.Request.Method, context.Request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")), requestBody, "\n");

                    // Create a MemoryStream to hold the request body content and rewind it
                    var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
                    requestBodyStream.Seek(0, SeekOrigin.Begin);

                    // Store the MemoryStream in HttpContext.Items for use in ValidateRequest
                    context.Items["OriginalRequestBodyStream"] = requestBodyStream;

                    // Replace the request body with the MemoryStream for further processing
                    request.Body = requestBodyStream;

                }

                if (!context.Response.HasStarted)
                {
                    await next(context);
                }

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }

        private async Task ValidateRequest(HttpContext httpContext)
        {
            try
            {
                var requestType = GetRequestType(httpContext);
                var contentType = httpContext.Request.ContentType?.ToLower();
                string typeName = requestType.FullName!;
                var assembly = requestType.Assembly;

                var originalRequestBodyStream = httpContext.Request.Body;

                using (var reader = new StreamReader(httpContext.Request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        Log.Information("Validating Request: {Method} {Path} {Time} {RequestBody} {NewLine}", httpContext.Request.Method, httpContext.Request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")), requestBody, "\n");

                        using (var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)))
                        {
                            httpContext.Items["OriginalRequestBodyStream"] = originalRequestBodyStream;
                            httpContext.Request.Body = requestBodyStream;

                            if (contentType!.Contains("application/x-www-form-urlencoded"))
                            {
                                var formData = QueryHelpers.ParseQuery(requestBody);
                                var requestInstance = Activator.CreateInstance(requestType);

                                foreach (var fieldName in formData.Keys)
                                {
                                    var propertyInfo = requestType.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                                    if (propertyInfo != null && propertyInfo.CanWrite)
                                    {
                                        var value = Convert.ChangeType(formData[fieldName][0], propertyInfo.PropertyType);
                                        propertyInfo.SetValue(requestInstance, value);
                                    }
                                }

                                httpContext.Items["RequestInstance"] = requestInstance;
                                await ValidateRequestObject(httpContext, requestInstance!);
                            }

                            if (contentType.Contains("application/json"))
                            {
                                await ValidateRequestObject(httpContext, JsonConvert.DeserializeObject(requestBody, requestType)!);
                            }

                        }

                        await next(httpContext);
                    }
                    else
                    {
                        await next(httpContext);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Type GetRequestType(HttpContext context)
        {
            var path = context.Request.Path.Value!.ToLowerInvariant();

            if (path.Contains("/api/textsummary/summarizetext"))
            {
                return typeof(TextSummaryRequest);
            }
            else if (path.Contains("/api/textsummary/summarizebatch"))
            {
                return typeof(TextSummaryRequest);
            }
            else
            {
                return typeof(Request);
            }

        }

        public async static Task ValidateRequestObject(HttpContext context, object request)
        {
            var validator = ValidatorResolver.ResolveValidator(request.GetType());
            var validationContext = new ValidationContext<object>(request);
            var validationResults = await validator.ValidateAsync(validationContext);

            if (!validationResults.IsValid)
            {
                await HandleValidationErrorsAsync(context, validationResults);
                return;
            }
        }

        public async static Task HandleValidationErrorsAsync(HttpContext context, FluentValidation.Results.ValidationResult validationResults)
        {
            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var Errors = validationResults.Errors.Select(e => new { Property = e.PropertyName, Message = e.ErrorMessage });
                StringBuilder stringBuilder = new StringBuilder();
                List<ProblemDetails> problemDetails = new();

                foreach (var error in Errors)
                {
                    stringBuilder.Append(error.Property);
                    stringBuilder.Append(" : ");
                    stringBuilder.Append(error.Message);

                    ProblemDetails errorResponse = new()
                    {
                        Title = "FluentValidation.ValidationException",
                        Status = context.Response.StatusCode,
                        Detail = stringBuilder.ToString(),
                        Instance = context.Request.Path,

                    };

                    problemDetails.Add(errorResponse);
                }

                var json = JsonConvert.SerializeObject(problemDetails);
                await context.Response.WriteAsync(json);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                Log.Information("Exception: {Method} {Path} {Time} {RequestBody} {ExceptionType} {NewLine}", context.Request.Method, context.Request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")), exception.GetType(), "\n");

                string errorId = Guid.NewGuid().ToString();
                var exceptionType = exception.GetType();
                var request = context.Request;
                var response = context.Response;
                response.Clear();
                string? message = string.Empty;
                var stackTrace = string.Empty;

                var errorUrl = $"/Error/HandleStatusCode/";

                switch (exceptionType.Name)
                {
                    case nameof(ValidationException):
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = $"{exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(BadRequestException):
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(KeyNotFoundException):
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(NotFoundException):
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(NotImplementedException):
                        response.StatusCode = (int)HttpStatusCode.NotImplemented;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(UnauthorizedAccessException):
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        message = exception.Message;
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(SqlException):
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(DbUpdateConcurrencyException):
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(NullReferenceException):
                        response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(InvalidOperationException):
                        response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(InvalidCastException):
                        response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;
                    case nameof(IOException):
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = $"{exceptionType.Name} {exception.Message}";
                        stackTrace = exception.StackTrace;
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = exception.Message;
                        stackTrace = exception.StackTrace;
                        break;
                }


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

                    await response.WriteAsync(JsonConvert.SerializeObject(ApiResponse<ProblemDetails>.Failure(problemDetails)));

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


        }

    }



}
