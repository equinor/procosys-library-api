using Microsoft.AspNetCore.Builder;

namespace Equinor.Procosys.Library.WebApi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void AddGlobalExceptionHandling(this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionHandler>();
    }
}
