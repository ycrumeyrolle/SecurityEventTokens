using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamMetadataProvider : IEventStreamMetadataService
    {
        private readonly EventStreamMetadataOptions _options;

        public EventStreamMetadataProvider(IOptions<EventStreamMetadataOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            for (int i = 0; i < _options.EventStreams.Count; i++)
            {
                var stream = _options.EventStreams[i];
                if (stream.MaxRetries == 0)
                {
                    stream.MaxRetries = int.MaxValue;
                }

                if (stream.MaxDeliveryTime == 0)
                {
               //     stream.MaxDeliveryTime = 10;
                }

                if (!string.Equals(stream.MethodUri, Constants.WebCallbackMethodUri, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Only the {nameof(EventStreamMetadata)}[{i}] with value of {Constants.WebCallbackMethodUri} is supported.", nameof(options));
                }
            }
        }

        public Task<IEnumerable<EventStreamMetadata>> GetAllAsync()
        {
            return Task.FromResult(_options.EventStreams.AsEnumerable());
        }

        public Task<EventStreamMetadata> GetEventStreamMetadataByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<EventStreamMetadata> GetEventStreamMetadataByIssuerAsync(string issuer)
        {
            return Task.FromResult(_options.EventStreams.FirstOrDefault(e => string.Equals(e.FeedUri, issuer, StringComparison.Ordinal)));
        }
    }
}
