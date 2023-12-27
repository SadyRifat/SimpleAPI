using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleAPI.Services
{
    public interface TokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
