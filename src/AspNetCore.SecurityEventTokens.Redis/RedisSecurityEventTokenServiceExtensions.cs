using System;
using AspNetCore.SecurityEventTokens;
using AspNetCore.SecurityEventTokens.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisSecurityEventTokenServiceExtensions
    {
        public static ISecurityEventTokenReceiverBuilder AddRedis(this ISecurityEventTokenReceiverBuilder builder)
        {
            return AddRedis(builder, o => { });
        }

        public static ISecurityEventTokenReceiverBuilder AddRedis(this ISecurityEventTokenReceiverBuilder builder, Action<RedisOptions> configure)
        {
            builder.Services.Configure(configure);
            builder.Services.AddSingleton<ISecurityEventTokenStore, RedisSecurityEventTokenStore>();
            return builder;
        }
    }
}
