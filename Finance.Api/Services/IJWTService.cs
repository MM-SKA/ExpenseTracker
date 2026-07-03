using Finance.Api.Models;

namespace Finance.Api.Services;

public interface IJWTService
{
    string GenerateToken(AppUser user);
}