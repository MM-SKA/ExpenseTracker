using Finance.Api.Application.DTOs.Expenses;

namespace Finance.Api.Application.DTOs.V2.Expenses;

public class ExpenseDtoV2
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public required string Currency { get; set; }
}
