using SimpleAPI.Dto.User;
using System.Security.Claims;

namespace SimpleAPI.Services
{
    public interface TokenService
    {
        Token GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
