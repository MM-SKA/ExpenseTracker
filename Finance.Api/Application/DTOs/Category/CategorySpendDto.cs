namespace Finance.Api.Application.DTOs.Category;

public class CategorySpendDto
{
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public int TransactionCount { get; set; }
}
