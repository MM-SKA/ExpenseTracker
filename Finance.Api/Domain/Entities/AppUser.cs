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
    public string Email { get; set; } = String.Empty;

    [Required]
    public string PasswordHash { get; set; } = String.Empty;

    [Required]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]

    public string PhoneNumber { get; set; } = String.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<Category> Categories { get; set; } = new List<Category>();

    [JsonIgnore]
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
