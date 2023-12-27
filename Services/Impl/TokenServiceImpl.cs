using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleAPI.Services.Impl
{
    public class TokenServiceImpl: TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenServiceImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var issuer = _configuration.GetValue<string>("Token:Issuer");
            var audience = _configuration.GetValue<string>("Token:Audience");
            var secretKey = _configuration.GetValue<string>("Token:SecretKey");
            var accessTokenExpirationMinutes = _configuration.GetValue<int>("Token:AccessTokenExpirationMinutes");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
