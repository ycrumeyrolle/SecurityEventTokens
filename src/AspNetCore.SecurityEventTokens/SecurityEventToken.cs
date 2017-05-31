using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    /// <summary>
    /// Represents a Security Event Token (SET) as defined in https://tools.ietf.org/html/draft-ietf-secevent-token-01.
    /// </summary>
    public class SecurityEventToken : JsonWebToken
    {
        public SecurityEventToken(IDictionary<string, JToken> claims, IDictionary<string, JToken> header = null)
            : base (claims, header)
        {
        }

        public JToken Events => GetClaim<JToken>("events");

        public string TransactionNumber => GetClaim<string>("txn");
        
        public static SecurityEventToken FromJwt(JsonWebToken jwt)
        {
            if (jwt == null)
            {
                throw new ArgumentNullException(nameof(jwt));
            }

            return new SecurityEventToken(jwt.Payload, jwt.Header);
        }
    }
}
