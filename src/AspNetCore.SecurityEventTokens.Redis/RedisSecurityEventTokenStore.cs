using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AspNetCore.SecurityEventTokens.Redis
{
    public class RedisSecurityEventTokenStore : ISecurityEventTokenStore, IDisposable
    {
        private readonly RedisOptions _options;
        private readonly ILogger _logger;
        private ConnectionMultiplexer _connection;
        private IDatabase _database;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public RedisSecurityEventTokenStore(ILoggerFactory loggerFactory, IOptions<RedisOptions> options)
        {
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<RedisSecurityEventTokenStore>();
        }

        public async Task<StoreStatus> TryStoreAsync(SecurityEventToken token)
        {
            await ConnectAsync();

            RedisKey key = ((RedisKey)token.Issuer).Append(token.JwtId);
            var result = await _database.StringGetSetAsync(token.JwtId, token.RawPayload);

            return result.IsNull ? StoreStatus.Stored : StoreStatus.Duplicated;
        }

        private async Task ConnectAsync()
        {
            if (_connection != null)
            {
                return;
            }

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection == null)
                {
                    var writer = new LoggerTextWriter(_logger);
                    _connection = await ConnectionMultiplexer.ConnectAsync(_options.Options, writer);
                    _database = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        private class LoggerTextWriter : TextWriter
        {
            private readonly ILogger _logger;

            public LoggerTextWriter(ILogger logger)
            {
                _logger = logger;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
            }

            public override void WriteLine(string value)
            {
                _logger.LogDebug(value);
            }
        }
    }
}