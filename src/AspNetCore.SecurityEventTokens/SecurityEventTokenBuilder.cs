using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityEventTokenBuilder
    {
        private readonly IDictionary<string, JToken> _claims = new Dictionary<string, JToken>();

        public Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

        public SecurityEventTokenBuilder JwtId(string value)
        {
            Claim("jti", value);
            return this;
        }

        public SecurityEventTokenBuilder Issuer(string value)
        {
            Claim("iss", value);
            return this;
        }

        public SecurityEventTokenBuilder Subject(string value)
        {
            Claim("sub", value);
            return this;
        }

        public SecurityEventTokenBuilder Audience(string value)
        {
            Claim("aud", value, canMerge: true);
            return this;
        }

        public SecurityEventTokenBuilder Audiences(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                Audience(value);
            }

            return this;
        }

        public SecurityEventTokenBuilder Events(JObject value)
        {
            Claim("events", value, canMerge: true);
            return this;
        }

        public SecurityEventTokenBuilder IssuedAt(DateTime value)
        {
            Claim("iat", value);
            return this;
        }

        public SecurityEventTokenBuilder NotBefore(DateTime value)
        {
            Claim("nbf", value);
            return this;
        }

        public SecurityEventTokenBuilder Expires(DateTime value)
        {
            Claim("exp", value);
            return this;
        }

        public SecurityEventTokenBuilder TransactionNumber(string value)
        {
            Claim("txn", value);
            return this;
        }

        public SecurityEventTokenBuilder Claim(string name, string value)
        {
            JValue jValue = JValue.CreateString(value);
            return Claim(name, jValue);
        }

        public SecurityEventTokenBuilder Claim(string name, IEnumerable<string> values)
        {
            var json = new JArray();
            foreach (var value in values)
            {
                json.Add(value);
            }

            Claim(name, json, canMerge: true);

            return this;
        }

        public SecurityEventTokenBuilder Claim(string name, IDictionary<string, JContainer> values)
        {
            var json = new JObject();
            foreach (var value in values)
            {
                json.Add(value.Key, value.Value);
            }

            Claim(name, json, canMerge: true);

            return this;
        }

        public SecurityEventTokenBuilder Claim(string name, JToken value, bool canMerge = false)
        {
            JToken token;
            if (_claims.TryGetValue(name, out token))
            {
                if (!canMerge)
                {
                    throw new InvalidOperationException();
                }

                JContainer jContainer = token as JContainer;
                if (jContainer != null)
                {
                    jContainer.Merge(value);
                }
                else
                {
                    JValue jValue = token as JValue;
                    if (jValue != null)
                    {
                        var jArray = new JArray(jValue, value);
                        _claims[name] = jArray;
                    }
                }
            }
            else
            {
                _claims[name] = value;
            }

            return this;
        }

        public SecurityEventTokenBuilder Claim(string name, DateTime value)
        {
            var epochTime = EpochTime.GetIntDate(value.ToUniversalTime());
            var claim = new JValue(epochTime);
            Claim(name, claim);

            return this;
        }

        public SecurityEventToken Build()
        {
            if (!_claims.ContainsKey("iss"))
            {
                throw new InvalidOperationException("No claim 'iss' is defined. This claim is required.");
            }

            if (!_claims.ContainsKey("jti"))
            {
                throw new InvalidOperationException("No claim 'jti' is defined. This claim is required.");
            }

            if (!_claims.ContainsKey("events"))
            {
                throw new InvalidOperationException("No claim 'events' is defined. This claim is required.");
            }

            if (!_claims.ContainsKey("iat"))
            {
                IssuedAt(UtcNow());
            }

            return new SecurityEventToken(_claims);
        }
    }
}
