using Finance.Api.Application.Interfaces;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(FinanceDbContext context) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        _ = context.RefreshTokens.Add(refreshToken);
        return Task.CompletedTask;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
        .FirstOrDefaultAsync(
        token => token.TokenHash == tokenHash,
        cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<List<RefreshToken>> GetByTokenFamilyIdAsync(string tokenFamilyId, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
        .Where(token => token.TokenFamilyId == tokenFamilyId)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
