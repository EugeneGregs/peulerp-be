using Microsoft.IdentityModel.Tokens;
using PeyulErp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeyulErp.Settings
{
    public static class UserExtensions
    {
        public static string GetUserToken(this User user, JwtSettings settings)
        {
            Console.WriteLine($"Key: {settings.Key}");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.ToString()),
                new Claim("status", user.Status.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                settings.Issuer,
                settings.Audience,
                claims,
                expires: DateTime.Now.AddHours(settings.ExpirationInHours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}