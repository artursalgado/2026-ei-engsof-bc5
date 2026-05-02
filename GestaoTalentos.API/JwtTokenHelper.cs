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
        if (user.Role == null)
            throw new ArgumentException("User role cannot be null");

        var claims = new List<Claim>
        {
            // ID do utilizador (sub é padrão JWT)
            new Claim("sub", user.Id.ToString()),

            // Username
            new Claim(ClaimTypes.Name, user.Username),

            // Role baseada na entidade Role (NÃO enum)
            new Claim(ClaimTypes.Role, user.Role.Nome)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}