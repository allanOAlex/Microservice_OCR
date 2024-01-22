using FluentValidation;
using ITT.DocumentUploadService.Application.Interfaces;
using ITT.DocumentUploadService.Application.IRepositories;
using ITT.DocumentUploadService.Application.IServices;
using ITT.DocumentUploadService.Domain.Entities;
using ITT.DocumentUploadService.Infrastructure.Interfaces;
using ITT.DocumentUploadService.Infrastructure.Repositories;
using ITT.DocumentUploadService.Infrastructure.Services;
using ITT.DocumentUploadService.Shared.Dtos.Requests;
using ITT.DocumentUploadService.Shared.Validations.ModelValidators;
using ITT.Shared.Application.IRepositories;
using ITT.Shared.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        private static IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        public class CorrelationIdEnricher : ILogEventEnricher
        {
            private const string CorrelationIdPropertyName = "CorrelationId";

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var correlationId = GetCorrelationId();
                var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

                logEvent.AddOrUpdateProperty(correlationIdProperty);
            }


            private string GetCorrelationId()
            {
                // Retrieve the correlation ID from the HTTP context if available
                var httpContext = httpContextAccessor.HttpContext;
                var correlationId = httpContext?.Request.Headers["CorrelationId"].FirstOrDefault();

                // If the correlation ID is not available in the HTTP context, generate a new GUID
                if (string.IsNullOrEmpty(correlationId))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Guid.NewGuid().ToString());
                    sb.Append('-');
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
                    correlationId = sb.ToString();
                }

                return correlationId;
            }


        }

        public class IPAddressEnricher1 : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var ipAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                if (ipAddress != null)
                {
                    var ipAddressProperty = propertyFactory.CreateProperty("IPAddress", ipAddress);
                    logEvent.AddPropertyIfAbsent(ipAddressProperty);
                }
            }
        }

        public class IPAddressEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var httpContext = httpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext?.Connection?.RemoteIpAddress;

                // Check if the IP address is available and not in IPv6 loopback format
                if (remoteIpAddress != null && !IPAddress.IsLoopback(remoteIpAddress))
                {
                    var ipAddress = remoteIpAddress.ToString();
                    var ipAddressProperty = propertyFactory.CreateProperty("IPAddress", ipAddress);
                    logEvent.AddPropertyIfAbsent(ipAddressProperty);
                }
            }
        }

        private static string GetLogFilePath(IConfiguration configuration)
        {
            string logFilePath = configuration["Logging:Serilog:LogFile"]!;
            string logFileName = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}.txt";
            var filePath = Path.Combine(logFilePath, logFileName);
            return filePath;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                //using var serviceProvider = services.BuildServiceProvider();
                //var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                services.AddScoped<IFileRepository, FileRepository>();



                services.AddScoped<IServiceManager, ServiceManager>();
                services.AddScoped<IFileService, FileService>();



                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Serilog", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With(new CorrelationIdEnricher())
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.With<IPAddressEnricher>()
                .Filter.ByExcluding(logEvent =>
                {
                    if (logEvent.Properties.TryGetValue("SourceContext", out var value) &&
                        value is ScalarValue scalarValue &&
                        scalarValue.Value is string sourceContext)
                    {
                        return sourceContext.StartsWith("Microsoft.");
                    }

                    return false;
                })
                .WriteTo.Async(s => s.Console(new CompactJsonFormatter()))
                .WriteTo.Async(s => s.File(new JsonFormatter(), configuration["Logging:Serilog:LogFile"]!, rollingInterval: RollingInterval.Day))
                .CreateLogger();

                services.AddSingleton<ILoggerFactory>(provider =>
                {
                    return new SerilogLoggerFactory(Log.Logger, true);
                });

                services.AddMemoryCache();

                return services;
            }
            catch (Exception)
            {

                throw;
            }



        }

        public static IServiceCollection AddValidatorServices(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddScoped<IValidator<FileUploadRequest>, FileUploadRequestValidator>();
                

                return services;
            }
            catch (Exception)
            {

                throw;
            }



        }

    }


}
