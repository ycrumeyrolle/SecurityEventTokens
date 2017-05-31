using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public interface ISecurityEventTokenDispatcher
    {
        Task NotifyAsync(SecurityEventToken token);
    }
}