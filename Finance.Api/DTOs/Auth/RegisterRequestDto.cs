using System.ComponentModel.DataAnnotations;

namespace Finance.Api.DTOs.Auth;

public class RegisterRequestDto{
    [Required]
    public string FullName { get; set; } = String.Empty;

    [Required]
    public string Email { get; set; } = String.Empty;

    [Required]
    public string Password { get; set; } = String.Empty;
}