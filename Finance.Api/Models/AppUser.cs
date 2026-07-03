using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Models;

public class AppUser{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public String FullName { get; set; } = String.Empty;

    [Required]
    [MaxLength(100)]
    public String Email { get; set; } = String.Empty;

    [Required]
    public String PasswordHash { get; set; } = String.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}