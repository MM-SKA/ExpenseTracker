using System.ComponentModel.DataAnnotations;

using Finance.Api.Application.Validation;

namespace Finance.Api.Application.DTOs.Expenses;

public class CreateExpenseDto
{
    [Required]
    [MaxLength(200)]
    // [JsonPropertyName("Notes")]
    public string Notes { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public required string CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public string? Location { get; set; }
}
