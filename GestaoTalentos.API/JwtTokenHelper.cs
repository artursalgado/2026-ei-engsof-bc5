using GestaoTalentos.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoTalentos.API;

public static class JwtTokenHelper
{
    public static string GenerateToken(User user, string key, string issuer, int expiryMinutes = 120)
    {
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("name", user.Username),
            new Claim("role", user.Role.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
