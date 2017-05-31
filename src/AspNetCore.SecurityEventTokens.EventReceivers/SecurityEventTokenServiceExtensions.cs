using System;
using AspNetCore.SecurityEventTokens;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityEventTokenServiceExtensions
    {
        public static ISecurityEventTokenReceiverBuilder AddSecurityEventTokenReceiver(this IServiceCollection services, Action<EventStreamMetadataOptions> configureOptions)
        {
            services.AddOptions();
            services.AddTransient<ISecurityEventTokenHandler, DefaultSecurityEventTokenHandler>();
            services.AddSingleton<IJwtSerializer, DefaultJwtSerializer>();
            services.AddSingleton<IEventStreamMetadataService, EventStreamMetadataProvider>();

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            return new SecurityEventTokenReceiverBuilder(services);
        }
    }
}
