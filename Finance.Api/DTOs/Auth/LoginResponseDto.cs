namespace Finance.Api.DTOs.Auth;

public class LoginResponseDto
{
    public string Message { get; set; } = string.Empty;
    public AuthDto User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}
