using Finance.Api.Application.Constants;

namespace Finance.Api.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public string? ErrorCode { get; set; }
}
