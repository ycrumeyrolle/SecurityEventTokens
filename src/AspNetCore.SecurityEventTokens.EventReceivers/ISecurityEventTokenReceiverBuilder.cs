using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.SecurityEventTokens
{
    public interface ISecurityEventTokenReceiverBuilder
    {
        IServiceCollection Services { get; }
    }
}