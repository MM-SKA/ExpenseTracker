using Microsoft.Extensions.Logging;

namespace Finance.Api.Presentation.Middleware.Logs;

internal static partial class ExceptionHandlingMiddlewareLogs
{
    [LoggerMessage(
        eventId: 6001,
        level: LogLevel.Warning,
        message: "Unhandled Exception. Method={Method}, Path={Path}"
    )]
    public static partial void UnhandledException(ILogger logger, Exception ex,string Method, string Path);

    [LoggerMessage(
        eventId: 6002,
        level: LogLevel.Warning,
        message: "Database Exception Occurred. Method={Method}, Path={Path}"
    )]
    public static partial void DbException(ILogger logger, Exception ex, string Method, string Path);

    [LoggerMessage(
        eventId: 6003,
        level: LogLevel.Warning,
        message: "Argument Null Exception Occurred. Method={Method}, Path={Path}"
    )]
    public static partial void ArgumentNullException(ILogger logger, Exception ex, string Method, string Path);

    [LoggerMessage(
        eventId: 6004,
        level: LogLevel.Critical,
        message: "Database Connection Failed : DNS Error"
    )]
    public static partial void NpgsqlException(ILogger logger, Exception ex);
}
