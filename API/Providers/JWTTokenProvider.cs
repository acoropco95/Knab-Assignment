using Knab.Assignment.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Knab.Assignment.API.Providers
{
    public interface IJWTTokenProvider
    {
        string GenerateToken(User user);
    }

    public class JWTTokenProvider : IJWTTokenProvider
    {
        private readonly IConfiguration _configuration;

        public JWTTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("User needs to have an email.");

            var claims = new List<Claim> {
                new(ClaimTypes.Email, user.Email)
            };

            var securityKey = _configuration.GetSection("AppSettings:JWTSecurityKey").Value;
            if (string.IsNullOrEmpty(securityKey))
                throw new Exception("Security key is missing in appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
