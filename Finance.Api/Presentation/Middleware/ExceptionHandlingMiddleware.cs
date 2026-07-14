using System.Text.Json;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{ 
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next , ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger=logger;
        _next=next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex , "Unhandeled Exception Occured");
            context.Response.StatusCode=500;
            context.Response.ContentType="application/json";
            var response = new ApiResponse{Success=false , Message="An unexpected error occured"};
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

}