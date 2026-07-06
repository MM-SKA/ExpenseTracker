using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.DTOs.Expenses;

public class CreateExpenseDto
{
    [Required]
    [MaxLength(200)]
    // [JsonPropertyName("Notes")]
    public string Notes { get; set; } = null!;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount is required and must be a positive value.")]
    public decimal Amount { get; set; }
    
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}