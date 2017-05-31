using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public interface IJwtSerializer
    {
        Task<string> SerializeAsync(JsonWebToken token);

        Task<JwtParsingResult> DeserializeAsync(string token);
    }
}