using ITT.DocumentUploadService.Api.Middleware;

namespace ITT.DocumentUploadService.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomMiddleware(this IApplicationBuilder app)
        {

            app.UseMiddleware<ApiRequestMiddleware>();
           
        }
    }
}
