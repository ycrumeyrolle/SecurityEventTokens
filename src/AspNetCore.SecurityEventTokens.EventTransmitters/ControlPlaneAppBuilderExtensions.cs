using AspNetCore.SecurityEventTokens;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class ControlPlaneAppBuilderExtensions
    {
        public static IApplicationBuilder UseControlPlane(this IApplicationBuilder app, ControlPlaneOptions options)
        {
            var eventStreamTemplate = options.Path + "/{id:string:required}";
            app.UseMiddleware<ControlPlaneMiddleware>(options);
            app.UseRouter(router => router.MapRoute(eventStreamTemplate, appBuilder => app.UseMiddleware<ControlPlaneMiddleware>()));
            return app;
        }
    }
}
