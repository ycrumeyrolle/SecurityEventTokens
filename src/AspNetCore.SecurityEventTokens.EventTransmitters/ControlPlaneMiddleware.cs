using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace AspNetCore.SecurityEventTokens
{
    public class ControlPlaneMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ControlPlaneOptions _options;
        private readonly IEventStreamMetadataService _eventStreamMetadataProvider;

        public ControlPlaneMiddleware(RequestDelegate next, IOptions<ControlPlaneOptions> options, IEventStreamMetadataService eventStreamMetadataProvider)
        {
            _next = next;
            _options = options.Value;
            _eventStreamMetadataProvider = eventStreamMetadataProvider;
        }
        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            if (request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            var id = (string)context.GetRouteValue("id");
            if (string.IsNullOrEmpty(id))
            {
                await _next(context);
                return;
            }

            if (HttpMethods.IsGet(request.Method))
            {
                var eventStream = await _eventStreamMetadataProvider.GetEventStreamMetadataByIdAsync(id);
                if (eventStream == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                var serializedEventStream = JsonConvert.SerializeObject(eventStream);
                
                await context.Response.WriteAsync(serializedEventStream);
            }
        }
    }
}