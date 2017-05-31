using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public interface ISecurityEventTokenHandler
    {
        Task<SecurityTokenEventResult> HandleSecurityEventTokenAsync(string token);
    }
}
