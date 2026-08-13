using Finance.Api.Application.Interfaces;

using Quartz;

namespace Finance.Api.Infrastructure.Jobs;

public sealed class RefreshTokenCleanupJob(
    IRefreshTokenRepository refreshTokenRepository, ILogger<RefreshTokenCleanupJob> logger)
    : IJob
{
    public async Task Execute(
        IJobExecutionContext context)
    {
        var expiredTokens =
            await refreshTokenRepository
                .GetExpiredTokensAsync(
                    context.CancellationToken).ConfigureAwait(false);

        logger.LogInformation(
            "Cleanup Job Started. Found {Count} expired tokens.",
        expiredTokens.Count);

        if (expiredTokens.Count == 0)
        {
            return;
        }

        await refreshTokenRepository
            .RemoveRangeAsync(
                expiredTokens,
                context.CancellationToken).ConfigureAwait(false);

        await refreshTokenRepository
            .SaveChangesAsync(
                context.CancellationToken).ConfigureAwait(false);

        logger.LogInformation(
            "Cleanup Job Completed.");
    }
}
