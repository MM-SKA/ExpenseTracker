using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Auth;

public class ChangePasswordDto
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    public required string NewPassword { get; set; }
}
