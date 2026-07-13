using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Auth;

public class LoginRequestDto{
    [Required]
    public string Email { get; set; } = String.Empty;

    [Required]
    public string Password { get; set; } = String.Empty;
}