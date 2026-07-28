using Finance.Api.Application.Interfaces;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Repositories;

internal sealed class AuthRepository(
    FinanceDbContext context)
    : IAuthRepository
{
    public async Task<bool> EmailExistsAsync(
    string email,
    CancellationToken cancellationToken)
    {
        var users =
            await context.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .Take(1)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return users.Count > 0;
    }

    public async Task<bool> PhoneExistsAsync(
    string phoneNumber,
    CancellationToken cancellationToken)
    {
        var users =
            await context.Users
                .AsNoTracking()
                .Where(u => u.PhoneNumber == phoneNumber)
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .Take(1)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return users.Count > 0;
    }

    public async Task AddUserAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        _ = await context.Users.AddAsync(
            user,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<AppUser?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await context.Users
            .FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken).ConfigureAwait(false);
    }

    public async Task<AppUser?> GetUserByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await context.Users
            .FirstOrDefaultAsync(
                u => u.Id == id,
                cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        _ = await context.SaveChangesAsync(
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<AppUser>> GetUsersAsync(
    CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
