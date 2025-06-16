using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Services
{
    public class Login_service
    {
        public static string GenerateJWTToken(User_Model user, IConfiguration config)
        {
            var secretKey = config["JwtSettings:Key"];
            var issuer = config["JwtSettings:Issuer"];
            var audience = config["JwtSettings:Audience"];
            var expiryMinutes = int.Parse(config["JwtSettings:ExpiryMinutes"]);

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("user_id", user.user_id.ToString()),
                new Claim(ClaimTypes.Email, user.email)
            }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
