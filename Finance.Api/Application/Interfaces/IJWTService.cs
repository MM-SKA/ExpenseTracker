using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IJWTService
{
    string GenerateToken(AppUser user);
}
