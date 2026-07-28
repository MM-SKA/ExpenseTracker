using Microsoft.Extensions.Logging;

namespace Finance.Api.Application.Logs;

internal static partial class CategoryServiceLogs
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Attempt to create category with existing name")]
    public static partial void DuplicateCategory(
        ILogger logger);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "New category added by UserId: {UserId}")]
    public static partial void CategoryCreated(
        ILogger logger,
        string userId);
}
