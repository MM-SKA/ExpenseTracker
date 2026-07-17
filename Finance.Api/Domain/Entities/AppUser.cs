using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Domain.Entities;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string FullName { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]

    public required string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<Category> Categories { get; set; } = [];

    [JsonIgnore]
    public ICollection<Expense> Expenses { get; set; } = [];
}
