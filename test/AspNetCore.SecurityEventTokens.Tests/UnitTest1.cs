using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AspNetCore.SecurityEventTokens.Tests
{
    public class UnitTest1
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

        [Fact]
        public void JoseVsWilson()
        {
            var eventStream = new EventStreamMetadata
            {
                FeedJwk = signingKey,
                FeedUri = "https://server.example.com",
                DeliveryUri = "http://localhost:37685/Events",
                Audiences = new[] { "s6BhdRkqt3" }
            };
            var token = new SecurityEventTokenBuilder()
               .JwtId(Guid.NewGuid().ToString())
               .Issuer("https://server.example.com")
               .Audience("s6BhdRkqt3")
               .IssuedAt(DateTime.UtcNow)
               .Claim("sid", "08a5019c-17e1-4977-8f42-65a12843ea02")
               .Events(new JObject(new { ok = true, array = new string[] { "hello", "world" } }))
               .Build();

            //var wilsonSerializer = new DefaultJwtSerializer();
            //var wilsonJwt = wilsonSerializer.Serialize(token, eventStream);
            //CngKey.Create(CngAlgorithm.ECDsaP384, )
            //var joseJwt = Jose.JWT.Encode(token.Claims,  );
        }
    }
}
