using Microsoft.Extensions.Logging;

namespace Finance.Api.Presentation.Middleware.Logs;

internal static partial class RequestLoggingMiddlewareLogs
{
    [LoggerMessage(
        eventId: 5001,
        level: LogLevel.Information,
        message: "Incoming Request. Method={Method} , Path={Path}"
    )]
    public static partial void IncomingRequest(ILogger logger, string method, string path);

    [LoggerMessage(
        eventId: 5002,
        level: LogLevel.Information,
        message: "Request Completed. Method={Method} , Path={Path} , StatusCode={StatusCode} , Duration={Duration}ms"
    )]
    public static partial void Requestcompleted(ILogger logger, string method, string path, int StatusCode, TimeSpan Duration);
}
