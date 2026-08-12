using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IJWTService
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
    string HashToken(string token);
}
