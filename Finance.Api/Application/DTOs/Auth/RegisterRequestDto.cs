using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Auth;

public class RegisterRequestDto{
    [Required]
    public string FullName { get; set; } = String.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = String.Empty;

    [Required]
    public string Password { get; set; } = String.Empty;

    [Required]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
    public string PhoneNumber { get; set; } = String.Empty;
}