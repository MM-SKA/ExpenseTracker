using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finance.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Finance.Api.Services;

public class JWTService : IJWTService {
    private readonly IConfiguration _config;

    public JWTService (IConfiguration config){
        _config=config;
    }

    public string GenerateToken(AppUser user){
        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
            
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JWTSecurityToken(
            issuer:_config["Jwt:Issuer"],
            audience:_config["Jwt:Audience"],
            claims:claims,
            expires:DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
            signingCredentials:creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}