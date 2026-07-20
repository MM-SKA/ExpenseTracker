using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Globalization;

using Finance.Api.Domain.Entities;
using Finance.Api.Application.Interfaces;

using Microsoft.IdentityModel.Tokens;

namespace Finance.Api.Infrastructure.Services;

public class JWTService(IConfiguration config) : IJWTService
{
    public string GenerateToken(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
        };

        var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is required.");
        var issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer configuration is required.");
        var audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience configuration is required.");
        var expiryMinutesText = config["Jwt:ExpiryMinutes"] ?? throw new InvalidOperationException("Jwt:ExpiryMinutes configuration is required.");
        var expiryMinutes = int.Parse(expiryMinutesText, CultureInfo.InvariantCulture);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
