using ITT.SummaryService.Api.Middleware;

namespace ITT.SummaryService.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomMiddleware(this IApplicationBuilder app)
        {

            app.UseMiddleware<ApiRequestMiddleware>();
            app.UseMiddleware<ModelValidationMiddleware>();

        }
    }
}
