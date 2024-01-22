using FluentValidation;
using ITT.Shared.Shared.Dtos.Common;
using ITT.Shared.Shared.Exceptions;
using ITT.SummaryService.Shared.Common;
using ITT.SummaryService.Shared.Dtos.Requests;
using ITT.SummaryService.Shared.Validations.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Reflection;
using System.Text;

namespace ITT.SummaryService.Api.Helpers.MiddlewareHelpers
{
    public static class MiddlewareHelper
    {
        public static bool IsRegistrationRequest(string path)
        {
            return path.Contains("/register", StringComparison.OrdinalIgnoreCase);
        }

        public static Task<string> RemoveDuplicatesFromRequestPath(HttpContext context)
        {
            try
            {
                string inputString = context.Request.Path.Value!.ToLower();
                string result = string.Empty;

                string[] parts = inputString.Split('/');

                if (parts.Length > 1 && parts.Distinct().Count() != parts.Length)
                {
                    string[] uniqueParts = parts.Distinct().ToArray();
                    result = string.Join("/", uniqueParts);
                    context.Request.Path = new PathString(result);


                }

                return Task.FromResult(result);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async static Task HandleRequestValidation(HttpContext context, RequestDelegate next)
        {
            var requestType = GetRequestType(context);

            var contentType = context.Request.ContentType?.ToLower();
            var requestBody = await GetRequestBody(context.Request);

            if (string.IsNullOrWhiteSpace(contentType))
            {
                await next(context);
                return;
            }

            if (contentType.Contains("application/json"))
            {
                var request = JsonConvert.DeserializeObject(requestBody, requestType);
                await ValidateRequestObject(context, request!);
            }
            else if (contentType.Contains("application/x-www-form-urlencoded"))
            {
                var formCollection = await context.Request.ReadFormAsync();
                var request = CreateRequestObjectFromForm(formCollection, requestType);
                await ValidateRequestObject(context, request);
            }
            else
            {
                // Unsupported content type
                context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                await context.Response.WriteAsync("Unsupported media type");
            }
        }

        public static object CreateRequestObjectFromForm(IFormCollection formCollection, Type requestType)
        {
            var request = Activator.CreateInstance(requestType);

            foreach (var property in requestType.GetProperties())
            {
                if (formCollection.TryGetValue(property.Name, out var value))
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(request, convertedValue);
                }
            }

            return request!;
        }

        private static void GetUrlEncodedFormData<T>(T request, out string httpRequestBody, out StreamWriter writer)
        {
            var formData = new Dictionary<string, string>();

            // Use reflection to retrieve the properties of the request object
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(request)?.ToString();
                var encodedKey = Uri.EscapeDataString(property.Name);
                var encodedValue = Uri.EscapeDataString(value ?? string.Empty);
                formData[encodedKey] = encodedValue;
            }

            httpRequestBody = string.Join("&", formData.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            // Create a new MemoryStream and write the URL-encoded form data to it
            var memoryStream = new MemoryStream();
            writer = new StreamWriter(memoryStream);
            writer.Write(httpRequestBody);
            writer.Flush();
            memoryStream.Position = 0;
        }

        public async static Task ValidateRequest(HttpContext httpContext, RequestDelegate next)
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
                        using (var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)))
                        {
                            httpContext.Items["OriginalRequestBodyStream"] = originalRequestBodyStream;
                            httpContext.Request.Body = requestBodyStream;

                            Console.WriteLine($"Request Body: {requestBody}");

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

        private async static Task<string> GetRequestBody(HttpRequest request)
        {
            if (request.ContentLength == 0)
                return string.Empty;

            using var reader = new StreamReader(request.Body);
            return await reader.ReadToEndAsync();
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

        public async static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                string errorId = Guid.NewGuid().ToString();
                var exceptionType = exception.GetType();
                var request = context.Request;
                var response = context.Response;
                response.Clear();
                string? message = string.Empty;
                var stackTrace = string.Empty;

                var errorUrl = $"/Error/HandleStatusCode/";

                if (exception is FluentValidation.ValidationException validationException)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = $"{exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(FluentValidation.ValidationException))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = $"{exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(BadRequestException))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(KeyNotFoundException))
                {
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(NotFoundException))
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(NotImplementedException))
                {
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(UnauthorizedAccessException))
                {
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(DbUpdateConcurrencyException))
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(NullReferenceException))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(InvalidOperationException))
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(InvalidCastException))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else if (exceptionType.Name == nameof(IOException))
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = $"{exceptionType.Name} {exception.Message}";
                    stackTrace = exception.StackTrace;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    stackTrace = exception.StackTrace;
                }

                if (!response.HasStarted)
                {
                    ProblemDetails problemDetails = new ProblemDetails()
                    {
                        Title = exceptionType.Name,
                        Status = response.StatusCode,
                        Detail = message,
                        Instance = request.Path,

                    };

                    response.ContentType = "application/json";
                    await response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse<ProblemDetails>
                    {
                        Successful = false,
                        Message = message,
                        Data = problemDetails
                    }));

                }
                else
                {
                    Log.Warning("Can't write error response. Response has already started.");
                }


            }
            catch (Exception)
            {
                throw;
            }


        }



    }

}
