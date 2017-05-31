using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityEventTokenReceiverBuilder : ISecurityEventTokenReceiverBuilder
    {
        public SecurityEventTokenReceiverBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}