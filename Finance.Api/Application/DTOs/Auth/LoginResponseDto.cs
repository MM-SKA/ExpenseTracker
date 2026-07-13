namespace Finance.Api.Application.DTOs.Auth;

public class LoginResponseDto
{
    public AuthDto User { get; set; } = new();

    public string Token { get; set; } = string.Empty;
}