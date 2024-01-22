using FluentValidation;
using ITT.SummaryService.Api.Helpers.MiddlewareHelpers;
using ITT.SummaryService.Shared.Validations.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace ITT.SummaryService.Api.Middleware
{
    public class ModelValidationMiddleware
    {

        private readonly RequestDelegate next;
        private readonly IHttpContextAccessor httpContextAccessor;


        public ModelValidationMiddleware(RequestDelegate Next, IHttpContextAccessor HttpContextAccessor)
        {
            next = Next;
            httpContextAccessor = HttpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var path = request.Path;

            try
            {
                var requestType = MiddlewareHelper.GetRequestType(httpContext);
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

                        await next(httpContext);
                    }
                    else
                    {
                        await next(httpContext);
                    }


                }

            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType();

                Log.Error("Exception: {Exception} {Message} {Method} {Path} {Time} ", exceptionType, ex.Message, request.Method, request.Path, DateTime.Now.ToString("T", new CultureInfo("en-GB")));

                throw;
            }

        }

        private async Task ValidateRequestObject(HttpContext context, object request)
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

        private async Task HandleValidationErrorsAsync(HttpContext context, FluentValidation.Results.ValidationResult validationResults)
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

    }
}
