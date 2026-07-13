using Finance.Api.Models;

namespace Finance.Api.Application.Interfaces;

public interface IJWTService
{
    string GenerateToken(AppUser user);
}