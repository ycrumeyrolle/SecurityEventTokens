using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCore.SecurityEventTokens
{
    public interface ISecurityEventTokenStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns><c>true</c> if the token is successfully stored. <c>false</c> </returns>
        Task<StoreStatus> TryStoreAsync(SecurityEventToken token);
    }
}