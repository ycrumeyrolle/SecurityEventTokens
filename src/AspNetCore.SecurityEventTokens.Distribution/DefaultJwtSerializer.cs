using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AspNetCore.SecurityEventTokens
{
    public class DefaultJwtSerializer : IJwtSerializer
    {
        private readonly JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();
        private readonly IEventStreamMetadataService _eventStreamMetadataService;

        public DefaultJwtSerializer(IEventStreamMetadataService eventStreamMetadataService)
        {
            _eventStreamMetadataService = eventStreamMetadataService;
        }

        public async Task<JwtParsingResult> DeserializeAsync(string token)
        {
            if (!_jwtHandler.CanReadToken(token))
            {
                return JwtParsingResult.ParseError();
            }

            var set = _jwtHandler.ReadJwtToken(token);

            var eventStream = await _eventStreamMetadataService.GetEventStreamMetadataByIssuerAsync(set.Issuer);
            if (eventStream == null)
            {
                return JwtParsingResult.IssuerError();
            }

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = ConvertToSecurityKey(eventStream.FeedJwk),
                ValidAudiences = eventStream.Audiences,
                ValidIssuer = set.Issuer,
                ValidateLifetime = false
            };
            SecurityToken securityToken;
            _jwtHandler.ValidateToken(token, validationParameters, out securityToken);
            var jwtSecurityToken = (JwtSecurityToken)securityToken;
            var claims = new Dictionary<string, JToken>();
            foreach (var claim in jwtSecurityToken.Claims)
            {
                switch (claim.ValueType)
                {
                    case JsonClaimValueTypes.Json:
                        claims.Add(claim.Type, JObject.Parse(claim.Value));
                        break;
                    case JsonClaimValueTypes.JsonArray:
                        claims.Add(claim.Type, JArray.Parse(claim.Value));
                        break;
                    case ClaimValueTypes.String:
                    case ClaimValueTypes.Integer:
                        claims.Add(claim.Type, new JValue(claim.Value));
                        break;
                    default:
                        break;
                }
            }

            var securityEventToken = new JsonWebToken(claims);
            return JwtParsingResult.Success(securityEventToken);
        }

        public async Task<string> SerializeAsync(JsonWebToken token)
        {
            var eventStream = await _eventStreamMetadataService.GetEventStreamMetadataByIssuerAsync(token.Issuer);
            var claims = new List<Claim>();
            foreach (var item in token.Claims)
            {
                Claim claim;
                switch (item.Value.Type)
                {
                    case JTokenType.Object:
                        claim = new Claim(item.Key, item.Value.ToString(Formatting.None), JsonClaimValueTypes.Json);
                        break;
                    case JTokenType.Array:
                        claim = new Claim(item.Key, item.Value.ToString(Formatting.None), JsonClaimValueTypes.JsonArray);
                        break;
                    case JTokenType.String:
                        claim = new Claim(item.Key, item.Value.Value<string>());
                        break;
                    case JTokenType.Integer:
                        claim = new Claim(item.Key, item.Value.Value<int>().ToString(), ClaimValueTypes.Integer);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                claims.Add(claim);
            }

            JwtHeader jwtHeader;
            if (eventStream.ConfidentialJwk != null)
            {
                throw new NotSupportedException("Wilson JWT framework currently only support dir ");
                //                    jwtHeader = new JwtHeader(new EncryptingCredentials(eventStream.ConfidentialJwk, eventStream.ConfidentialJwk.Alg, SecurityAlgorithms.EcdsaSha512));
            }
            else if (eventStream.FeedJwk != null)
            {
                jwtHeader = new JwtHeader(new SigningCredentials(ConvertToSecurityKey(eventStream.FeedJwk), eventStream.FeedJwk.Alg));
            }
            else
            {
                jwtHeader = new JwtHeader();
            }

            var jwtPayload = new JwtPayload(claims);
            JwtSecurityToken jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);

            return _jwtHandler.WriteToken(jwtToken);
        }

        private static SecurityKey ConvertToSecurityKey(JsonWebKey key)
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.JsonWebKey
            {
                Alg = key.Alg,
                Crv = key.Crv,
                D = key.D,
                DP = key.DP,
                DQ = key.DQ,
                E = key.E,
                K = key.K,
                Kid = key.Kid,
                Kty = key.Kty,
                N = key.N,
                Oth = key.Oth,
                P = key.P,
                Q = key.Q,
                QI = key.QI,
                Use = key.Use,
                X = key.X,
                X5t = key.X5t,
                X5tS256 = key.X5tS256,
                X5u = key.X5u,
                Y = key.Y
            };
            foreach (var item in key.AdditionalData)
            {
                securityKey.AdditionalData.Add(item);
            }

            return securityKey;
        }
    }
}