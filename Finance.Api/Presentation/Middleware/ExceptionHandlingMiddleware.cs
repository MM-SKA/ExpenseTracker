using System.Text.Json;
using Npgsql;
using System.Net.Sockets;

using Finance.Api.Presentation.Middleware.Logs;

using Finance.Api.Application.DTOs.Common;

using Microsoft.EntityFrameworkCore;
namespace Finance.Api.Presentation.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (ArgumentNullException ex)
        {
            ExceptionHandlingMiddlewareLogs.ArgumentNullException(logger, ex, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new ApiResponse { Success = false, Message = "a null argument is being passed" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.InnerException is NpgsqlException)
            {
                context.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;

                await context.Response.WriteAsJsonAsync(
                new ApiResponse
                {
                    Success = false,
                    Message = "Database connection failed",
                    ErrorCode = "DB001"
                });

                return;
            }
            throw;
        }
        catch (NpgsqlException ex)
        {
            ExceptionHandlingMiddlewareLogs.NpgsqlException(logger, ex);

            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

            var message = "Database connection failed";

            var errorCode = "DB001";

            if (ex.InnerException is SocketException socketEx)
            {
                if (socketEx.SocketErrorCode == SocketError.HostNotFound)
                {
                    message = "Database DNS error";
                    errorCode = "DB002";
                }
            }

            var response = new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };

            await context.Response
            .WriteAsJsonAsync(response)
            .ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            ExceptionHandlingMiddlewareLogs.DbException(logger, ex, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new ApiResponse { Success = false, Message = "a database exception occurred" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ExceptionHandlingMiddlewareLogs.UnhandledException(logger, ex, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new ApiResponse { Success = false, Message = "an unhandled exception occurred" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
        }
    }
}
