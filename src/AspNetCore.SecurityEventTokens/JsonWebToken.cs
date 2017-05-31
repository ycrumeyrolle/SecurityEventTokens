using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public class JsonWebToken
    {
        private static readonly DateTime _epochDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private readonly IDictionary<string, JToken> _claims;
        private readonly IDictionary<string, JToken> _header;

        public JsonWebToken(IDictionary<string, JToken> claims, IDictionary<string, JToken> header = null)
        {
            _claims = claims;
            _header = header ?? new Dictionary<string, JToken>();
        }

        public IDictionary<string, JToken> Claims => _claims;

        public IDictionary<string, JToken> Header => _header;

        public string JwtId => GetClaim<string>("jti");

        public string Issuer => GetClaim<string>("iss");

        public IEnumerable<string> Audiences => GetClaimList<string>("aud");

        public string Subject => GetClaim<string>("sub");

        public DateTime? IssuedAt => ToDateTime(GetClaim<int?>("iat"));

        public DateTime? NotBefore => ToDateTime(GetClaim<int?>("nbf"));

        public DateTime? ExpirationTime => ToDateTime(GetClaim<int?>("exp"));

        public string Type => GetHeader<string>("typ");

        public string ContentType => GetHeader<string>("cty");

        protected TValue GetClaim<TValue>(string name)
        {
            if (_claims.TryGetValue(name, out var claim))
            {
                return claim.Value<TValue>();
            }

            return default(TValue);
        }

        protected IEnumerable<TValue> GetClaimList<TValue>(string name)
        {
            if (_claims.TryGetValue(name, out var claim))
            {
                return claim.Values<TValue>();
            }

            return Enumerable.Empty<TValue>();
        }

        protected TValue GetHeader<TValue>(string name)
        {
            if (_header.TryGetValue(name, out var header))
            {
                return header.Value<TValue>();
            }

            return default(TValue);
        }

        protected static DateTime ToDateTime(long intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;
            return _epochDate.AddTicks(timeInTicks);
        }

        protected static DateTime? ToDateTime(long? intDate)
        {
            if (intDate.HasValue)
            {
                return ToDateTime(intDate.Value);
            }

            return null;
        }
    }
}
