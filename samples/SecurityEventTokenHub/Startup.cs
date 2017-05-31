using AspNetCore.SecurityEventTokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SecurityEventTokenHubSample
{
    public class Startup
    {
        private static readonly JsonWebKey key = new JsonWebKey("{" +
"\"kty\": \"EC\"," +
"\"d\": \"XwZQ8DDA9sBbXBwhFPtIdIgUaQ6FfwglSJ71lPuNXDk\"," +
"\"use\": \"sig\"," +
"\"crv\": \"P-256\"," +
"\"x\": \"rJLq1-gkobvPSOw0QYL57WNsUeXKXYBsll2wTtuC3Zg\"," +
"\"y\": \"Ib_rwFYWZAtiKbL4aP2tf6H6MdD89cBLlcZ73clGVgk\"," +
"\"alg\": \"ES256\"" +
"}");

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSecurityEventTokenReceiver(o =>
                {
                    o.EventStreams = new[]
                    {
                        new EventStreamMetadata
                        {
                            FeedJwk = key,
                            FeedUri = "https://server.example.com",
                            Audiences = new [] { "s6BhdRkqt3" }
                        }
                    };
                })
                .AddInMemoryDispatcher()
                .AddInMemoryStore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSecurityEventTokenReceiver(new SecurityEventTokenReceiverOptions
            {
                Path = new PathString("/Events")
            });
        }
    }

}
