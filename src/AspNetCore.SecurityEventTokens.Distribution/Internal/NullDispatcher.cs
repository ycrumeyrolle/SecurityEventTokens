using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCore.SecurityEventTokens
{
    public class NullDispatcher : ISecurityEventTokenDispatcher
    {
        private readonly ILogger<NullDispatcher> _logger;

        public NullDispatcher(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NullDispatcher>();
        }

        public Task NotifyAsync(SecurityEventToken token)
        {
            _logger.LogInformation($"Token {token.JwtId} is published");
            return Task.CompletedTask;
        }
    }
}
