using System;
using System.Threading.Tasks;
using AspNetCore.SecurityEventTokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EventTransmitterSample
{
    class Program
    {
        private static readonly JsonWebKey signingKey = new JsonWebKey("{" +
  "\"kty\": \"EC\"," +
  "\"d\": \"XwZQ8DDA9sBbXBwhFPtIdIgUaQ6FfwglSJ71lPuNXDk\"," +
  "\"use\": \"sig\"," +
  "\"crv\": \"P-256\"," +
  "\"kid\": \"1\"," +
  "\"x\": \"rJLq1-gkobvPSOw0QYL57WNsUeXKXYBsll2wTtuC3Zg\"," +
  "\"y\": \"Ib_rwFYWZAtiKbL4aP2tf6H6MdD89cBLlcZ73clGVgk\"," +
  "\"alg\": \"ES256\"" +
"}");
        private static readonly JsonWebKey encryptionKey = new JsonWebKey("{" +
  "\"kty\": \"EC\"," +
  "\"d\": \"6DzLDAmVWgHtRMEDiJ8eTmy7qjfaVDsqmgsLCX_XLtY\"," +
  "\"use\": \"enc\"," +
  "\"crv\": \"P-256\"," +
  "\"kid\": \"2\"," +
  "\"x\": \"OYXwcFo78brDCMN7U-xgSzbfqzPwqWlNWU4fQjkyLzk\"," +
  "\"y\": \"BsVoL9aZB-J5-j1QAN3iPqhulDOmyyFbT2TJeYuyGOM\"," +
  "\"alg\": \"ES256\"" +
"}");

        static void Main(string[] args)
        {
            var options = new EventStreamMetadataOptions
            {
                EventStreams = new[]
                {
                    new EventStreamMetadata
                    {
                        FeedJwk = signingKey,
                        FeedUri = "https://server.example.com",
                        DeliveryUri = "http://localhost:37685/Events",
                        Audiences = new [] { "s6BhdRkqt3" }
                    }
                }
            };
            var loggerFactory = new LoggerFactory()
                    .AddDebug(LogLevel.Trace)
                    .AddConsole(LogLevel.Trace);
            var eventStreamMetadataProvider = new EventStreamMetadataProvider(Options.Create(options));
            var transmitter = new EventTransmitter(loggerFactory, new DefaultJwtSerializer(eventStreamMetadataProvider), eventStreamMetadataProvider);

            Parallel.For(0, 10, i =>
            {
                while (true)
                {
                    SecurityEventToken token = new SecurityEventTokenBuilder()
                        .JwtId(Guid.NewGuid().ToString())
                        .Issuer("https://server.example.com")
                        .Audience("s6BhdRkqt3")
                        .ResetPasswordEvent("abcd")
                        .LogoutEvent()
                        .SessionId("08a5019c-17e1-4977-8f42-65a12843ea02")
                        .Build();
                    transmitter.TransmitAsync(token).Wait();
                }
            });
        }
    }
}