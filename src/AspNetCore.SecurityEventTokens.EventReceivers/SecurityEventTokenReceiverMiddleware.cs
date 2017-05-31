using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AspNetCore.SecurityEventTokens
{
    public class SecurityEventTokenReceiverMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityEventTokenReceiverOptions _options;
        private readonly ISecurityEventTokenHandler _handler;

        public SecurityEventTokenReceiverMiddleware(RequestDelegate next, IOptions<SecurityEventTokenReceiverOptions> options, ISecurityEventTokenHandler handler)
        {
            _next = next;
            _options = options.Value;
            _handler = handler;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            if (request.Path != _options.Path || !HttpMethods.IsPost(request.Method))
            {
                await _next(context);
                return;
            }

            if (request.ContentType != "application/jwt")
            {
                // bad content type
                return;
            }

            string value;
            using (var reader = new StreamReader(request.Body))
            {
                value = await reader.ReadToEndAsync();
            }

            var result = await _handler.HandleSecurityEventTokenAsync(value);
            switch (result.Status)
            {
                case SecurityTokenEventStatus.OK:
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    break;
                case SecurityTokenEventStatus.Verified:
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync("{\"challengeResponse\":\"" + result.ChallengeResponse + "\"}");
                    break;
                case SecurityTokenEventStatus.DuplicateSecurityEventToken:
                    context.Response.Headers["Content-Type"] = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(Errors.Duplicate);
                    break;
                case SecurityTokenEventStatus.SecurityEventTokenTypeError:
                    context.Response.Headers["Content-Type"] = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    throw new NotImplementedException();
                    break;
                case SecurityTokenEventStatus.SecurityEventTokenParseError:
                    context.Response.Headers["Content-Type"] = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(Errors.JwtParse);
                    break;
                case SecurityTokenEventStatus.SecurityEventTokenDataError:
                    context.Response.Headers["Content-Type"] = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    throw new NotImplementedException();
                    break;
                case SecurityTokenEventStatus.SecurityEventTokenIssuerError:
                    context.Response.Headers["Content-Type"] = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(Errors.JwtIssuer);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
