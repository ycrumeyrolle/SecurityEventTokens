using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public static class OidcSecurityEventTokenBuilderExtensions
    {
        private static readonly JObject LogoutObject = CreateLogoutObject();

        public static SecurityEventTokenBuilder LogoutEvent(this SecurityEventTokenBuilder builder)
        {
            return builder.Events(LogoutObject);
        }

        public static SecurityEventTokenBuilder SessionId(this SecurityEventTokenBuilder builder, string sessionId)
        {
            return builder.Claim("sid", sessionId);
        }

        private static JObject CreateLogoutObject()
        {
            var obj = new JObject();
            obj.Add("http://schemas.openid.net/event/backchannel-logout", new JObject());
            return obj;
        }
    }
}
