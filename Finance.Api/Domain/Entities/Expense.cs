using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Api.Domain.Entities;

public class Expense
{
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(250)]
    public string? Notes { get; set; }

    public int UserId { get; set; }

    public AppUser? User { get; set; }
}
