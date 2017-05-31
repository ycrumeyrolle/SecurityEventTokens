using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public static class VerifySecurityEventTokenBuilderExtensions
    {
        public static SecurityEventTokenBuilder VerifyEvent(this SecurityEventTokenBuilder builder, string confirmChallenge)
        {
            var obj = new JObject();
            obj.Add(Constants.VerifyEventName, JObject.FromObject(new { confirmChallenge }));
            builder.Events(obj);
            return builder;
        }
    }
}
