using System;
using AspNetCore.SecurityEventTokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SecurityEventTokenTransmitterServiceExtensions
    {
        public static IServiceCollection AddSecurityEventTokenTransmitter(this IServiceCollection services, Action<EventStreamMetadataOptions> configureOptions = null)
        {
            services.AddSingleton<IJwtSerializer, DefaultJwtSerializer>();
            services.AddSingleton<IEventStreamMetadataService, EventStreamMetadataProvider>();
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            return services;
        }
    }
}
