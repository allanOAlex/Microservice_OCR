using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using ITT.SummaryService.Api.Extensions;
using ITT.SummaryService.Infrastructure.Extensions;
using ITT.SummaryService.Persistence.DataContext;
using ITT.SummaryService.Shared.Common;
using ITT.SummaryService.Shared.Validations.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

string CORSPolicy = "OpenCORSPolicy";

var SSConnString = builder.Configuration.GetConnectionString("ConnString");
AppConstants.Conn = SSConnString;

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors(options =>
{
    options.AddPolicy(
      name: CORSPolicy,
      builder => {
          builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
      });
});

builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(SSConnString));

builder.Services.AddWebEncoders();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddValidatorsFromAssemblyContaining<SharedValidatorAssembly>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Text Summarization",
        Version = "v1.1",
        Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("WALTERIONS"),
        Contact = new OpenApiContact
        {
            Name = "WALTERIONS",
            Email = "https://walterions.io/",
        },

        License = new OpenApiLicense
        {
            Name = "WALTERIONS",
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "Bearer",

        Description = @"JWT Authorization header using the bearer scheme.
                    Enter 'Bearer' [Space] and then your token in the next inpout below.
                    Example: 'Bearer asddvsvs123'",

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },

            new string[] { }
        }
    });
});

builder.Services.AddControllers();
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddValidatorsFromAssemblyContaining<SharedValidatorAssembly>();
//builder.Services.AddValidatorServices(builder.Configuration);

var app = builder.Build();


app.UseCustomMiddleware();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Text Summarization v1.1"));
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(CORSPolicy);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
