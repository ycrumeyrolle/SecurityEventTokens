using AspNetCore.SecurityEventTokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InMemorySecurityEventTokenServiceExtensions
    {
        public static ISecurityEventTokenReceiverBuilder AddInMemoryStore(this ISecurityEventTokenReceiverBuilder builder)
        {
            builder.Services.AddSingleton<ISecurityEventTokenStore, InMemorySecurityEventTokenStore>();
            return builder;
        }

        public static ISecurityEventTokenReceiverBuilder AddInMemoryDispatcher(this ISecurityEventTokenReceiverBuilder builder)
        {
            builder.Services.AddSingleton<ISecurityEventTokenDispatcher, NullDispatcher>();
            return builder;
        }
    }
}
