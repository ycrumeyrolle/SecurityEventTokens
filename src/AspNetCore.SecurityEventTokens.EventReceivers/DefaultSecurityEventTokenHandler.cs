using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AspNetCore.SecurityEventTokens
{
    /// <summary>
    /// Implementation of <see cref="ISecurityEventTokenHandler"/> storing the event and publishing the the feeds.
    /// </summary>
    public class DefaultSecurityEventTokenHandler : ISecurityEventTokenHandler, IDisposable
    {
        private BlockingCollection<SecurityEventToken> _messageQueue;
        private readonly Task _outputTask;
        private readonly ISecurityEventTokenStore _eventStore;
        private readonly ISecurityEventTokenDispatcher _eventDispatcher;
        private readonly SecurityEventTokenReceiverOptions _options;
        private readonly IJwtSerializer _jwtSerializer;

        public DefaultSecurityEventTokenHandler(
            IOptions<SecurityEventTokenReceiverOptions> options, 
            ISecurityEventTokenStore eventStore, 
            ISecurityEventTokenDispatcher eventDispatcher, 
            IJwtSerializer jwtSerializer)
        {
            _options = options.Value;
            _eventStore = eventStore;
            _eventDispatcher = eventDispatcher;
            _jwtSerializer = jwtSerializer;
            if (_options.EnableHub)
            {
                _messageQueue = new BlockingCollection<SecurityEventToken>();
                _outputTask = Task.Factory.StartNew(
                   ProcessQueueAsync,
                   this,
                   TaskCreationOptions.LongRunning);
            }
        }

        public async Task<SecurityTokenEventResult> HandleSecurityEventTokenAsync(string token)
        {
            var result = await _jwtSerializer.DeserializeAsync(token);
             
            if (result.State == JwtParsingState.ParseError)
            {
                return SecurityTokenEventResult.ParseError();
            }

            if (result.State == JwtParsingState.IssuerError)
            {
                return SecurityTokenEventResult.IssuerError();
            }

            var securityEventToken = SecurityEventToken.FromJwt(result.Token);
            string challengeResponse;
            if (TryValidateVerifySecurityEventToken(securityEventToken, out challengeResponse))
            {
                return SecurityTokenEventResult.Verified(challengeResponse);
            }

            if (securityEventToken.Events == null || !securityEventToken.Events.HasValues)
            {
                return SecurityTokenEventResult.DataError();
            }

            if (_options.EnableHub)
            {
                _messageQueue.TryAdd(securityEventToken);
            }
            else
            {
                var storeStatus = await HandleSecurityEventTokenCoreAsync(securityEventToken);
                if (storeStatus == StoreStatus.Duplicated)
                {
                    return SecurityTokenEventResult.Duplicate();
                }
            }

            return SecurityTokenEventResult.OK();
        }

        private bool TryValidateVerifySecurityEventToken(SecurityEventToken token, out string challengeResponse)
        {
            challengeResponse = token.Events[Constants.VerifyEventName]?.Value<string>(Constants.VerifyConfirmChallenge);
            return !string.IsNullOrEmpty(challengeResponse);
        }
        
        private async Task ProcessQueueAsync()
        {
            foreach (var token in _messageQueue.GetConsumingEnumerable())
            {
                await HandleSecurityEventTokenCoreAsync(token);
            }
        }

        private async Task<StoreStatus> HandleSecurityEventTokenCoreAsync(SecurityEventToken token)
        {
            StoreStatus storeStatus = StoreStatus.Skipped;
            if (_eventStore != null)
            {
                storeStatus = await _eventStore.TryStoreAsync(token);
            }

            if (_eventDispatcher != null)
            {
                await _eventDispatcher.NotifyAsync(token);
            }

            return storeStatus;
        }

        private static async Task ProcessQueueAsync(object state)
        {
            var processor = (DefaultSecurityEventTokenHandler)state;
            await processor.ProcessQueueAsync();
        }

        public void Dispose()
        {
            if (_outputTask != null)
            {
                _messageQueue.CompleteAdding();

                try
                {
                    _outputTask.Wait(1500);
                }
                catch (TaskCanceledException) { }
                catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException) { }
            }
        }
    }
}
