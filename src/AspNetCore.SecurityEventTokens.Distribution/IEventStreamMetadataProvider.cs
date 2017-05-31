using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public interface IEventStreamMetadataService
    {
        Task<EventStreamMetadata> GetEventStreamMetadataByIssuerAsync(string issuer);

        Task<EventStreamMetadata> GetEventStreamMetadataByIdAsync(string id);
    }
}
