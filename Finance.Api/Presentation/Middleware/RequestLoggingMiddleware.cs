namespace Finance.Api.Presentation.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Console.WriteLine("REQUEST LOGGING MIDDLEWARE HIT");
        ArgumentNullException.ThrowIfNull(context);
        var startTime = DateTime.UtcNow;

        _logger.LogInformation(
            "Incoming Request. Method={Method}, Path={Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context).ConfigureAwait(false);

        var duration =
            DateTime.UtcNow - startTime;

        _logger.LogInformation(
            "Request Completed. Method={Method}, Path={Path}, StatusCode={StatusCode}, Duration={Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}
