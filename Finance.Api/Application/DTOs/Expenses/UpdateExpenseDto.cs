using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Application.DTOs.Expenses;

public class UpdateExpenseDto
{
    [Required]
    public string Notes { get; set; } = null!;

    [Required]
    [JsonPropertyName("Amount")]
    public decimal Amount { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
