using System.ComponentModel.DataAnnotations;

using Finance.Api.Application.Validation;

namespace Finance.Api.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    public required string FullName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
    public required string PhoneNumber { get; set; }
}