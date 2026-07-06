namespace Finance.Api.DTOs.Auth;

public class RegisterResponseDto
{
    public string Message { get; set; } = string.Empty;
    public AuthDto? User { get; set; }
}
