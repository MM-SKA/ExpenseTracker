using Finance.Api.Application.DTOs.Expenses;

namespace Finance.Api.Application.DTOs.V1.Expenses;

public class ExpenseDtoV1
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string CategoryId { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? Location { get; set; }
}
