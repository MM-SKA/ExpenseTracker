namespace Finance.Api.Application.DTOs.Expenses;

public class FilterExpenseDto
{
    public string? categoryId { get; set; }

    public DateTime? startDate { get; set; }

    public DateTime? endDate { get; set; }

    public decimal? minAmount { get; set; }

    public decimal? maxAmount { get; set; }

    public int? month { get; set; }

    public int? year { get; set; }

    public string? notes { get; set; }

    public bool includeAnalytics { get; set; }

    public string? Location { get; set; }
}
