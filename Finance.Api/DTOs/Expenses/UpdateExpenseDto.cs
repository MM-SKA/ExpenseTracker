using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.DTOs.Expenses;

public class UpdateExpenseDto
{
    [Required]
    [MaxLength(200)]
    [JsonPropertyName("Note")]
    public string Notes { get; set; } = null!;
    
    [Required]
    [JsonPropertyName("Amount")]
    public decimal Amount { get; set; }
    
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
