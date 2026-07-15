using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Services;

/// <summary>Wystawia tokeny JWT dla zalogowanych użytkowników.</summary>
public class TokenService
{
    private readonly IConfiguration _cfg;

    public TokenService(IConfiguration cfg) => _cfg = cfg;

    public string Create(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]
            ?? throw new InvalidOperationException("Brak klucza Jwt:Key w konfiguracji.")));

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            },
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
