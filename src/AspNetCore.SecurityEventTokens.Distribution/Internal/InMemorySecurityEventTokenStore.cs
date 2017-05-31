using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public class InMemorySecurityEventTokenStore : ISecurityEventTokenStore
    {
        private readonly ConcurrentDictionary<string, SecurityEventToken> _innerStore = new ConcurrentDictionary<string, SecurityEventToken>();

        public Task<StoreStatus> TryStoreAsync(SecurityEventToken token)
        {
            var status = _innerStore.TryAdd(token.JwtId, token);
            return Task.FromResult(status ? StoreStatus.Stored : StoreStatus.Duplicated);
        }
    }
}
