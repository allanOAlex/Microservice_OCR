using ITT.Client.Mvc.Middleware;

namespace ITT.Client.Mvc.Extensions
{
    public static class MIddlewareExtensions
    {
        public static void UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ClientRequestMiddleware>();

        }
    }
}
