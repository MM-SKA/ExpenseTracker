using System.Data.Common;
using System.Text.Json;

using Finance.Api.Presentation.Middleware.Logs;

using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Presentation.Middleware;

internal class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ArgumentNullException.ThrowIfNull(context);
            ExceptionHandlingMiddlewareLogs.UnhandledException(logger, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new ApiResponse { Success = false, Message = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
        }
    }
}
