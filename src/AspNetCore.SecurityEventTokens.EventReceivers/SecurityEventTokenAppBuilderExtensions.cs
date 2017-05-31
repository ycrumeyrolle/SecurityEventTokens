using AspNetCore.SecurityEventTokens;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityEventTokenAppBuilderExtensions
    {
        public static IApplicationBuilder UseSecurityEventTokenReceiver(this IApplicationBuilder app, SecurityEventTokenReceiverOptions options)
        {
            app.UseMiddleware<SecurityEventTokenReceiverMiddleware>(Options.Create(options));

            return app;
        }
    }
}
