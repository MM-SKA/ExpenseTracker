using Finance.Api.Presentation.Middleware.Logs;

namespace Finance.Api.Presentation.Middleware;

internal class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        // Console.WriteLine("REQUEST LOGGING MIDDLEWARE HIT");
        ArgumentNullException.ThrowIfNull(context);
        var startTime = DateTime.UtcNow;

        RequestLoggingMiddlewareLogs.IncomingRequest(logger, context.Request.Method, context.Request.Path);

        await next(context).ConfigureAwait(false);

        var duration =
            DateTime.UtcNow - startTime;

        RequestLoggingMiddlewareLogs.Requestcompleted(logger, context.Request.Method, context.Request.Path, context.Response.StatusCode, duration);
    }
}
