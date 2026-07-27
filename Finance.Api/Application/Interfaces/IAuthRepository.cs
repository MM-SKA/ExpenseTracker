using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IAuthRepository
{

    Task<int> GetNextUserIdAsync(
        CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken);

    Task<bool> PhoneExistsAsync(
        string phoneNumber,
        CancellationToken cancellationToken);

    Task AddUserAsync(
        AppUser user,
        CancellationToken cancellationToken);

    Task<AppUser?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<AppUser?> GetUserByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<List<AppUser>> GetUsersAsync(
        CancellationToken cancellationToken);
}
