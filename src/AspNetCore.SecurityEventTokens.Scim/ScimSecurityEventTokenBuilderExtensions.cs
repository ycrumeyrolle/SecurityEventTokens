using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public static class ScimSecurityEventTokenBuilderExtensions
    {
        public static SecurityEventTokenBuilder ResetPasswordEvent(this SecurityEventTokenBuilder builder, string id)
        {
            var obj = new JObject();
            obj.Add("urn:ietf:params:scim:event:passwordReset", JObject.FromObject(new { id }));
            builder.Events(obj);
            return builder;
        }
    }
}
