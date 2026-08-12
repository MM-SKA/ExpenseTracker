namespace Finance.Api.Application.DTOs.Auth;

public class LoginResponseDto
{
    public AuthDto User { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
