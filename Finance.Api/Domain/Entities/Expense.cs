using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Api.Domain.Entities;

public class Expense
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(250)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public string UserId { get; set; }

    public AppUser? User { get; set; }
}
