using Microsoft.Extensions.Logging;

namespace Finance.Api.Presentation.Middleware.Logs;

internal static partial class ExceptionHandlingMiddlewareLogs
{
    [LoggerMessage(
        eventId: 6001,
        level: LogLevel.Warning,
        message: "Unhandled Exception. Method={Method}, Path={Path}"
    )]
    public static partial void UnhandledException(ILogger logger, string Method, string Path);

    [LoggerMessage(
        eventId: 6002,
        level: LogLevel.Warning,
        message: "Unhandled Exception. Method={Method}, Path={Path}"
    )]
    public static partial void DbException(ILogger logger, string Method, string Path);
}
