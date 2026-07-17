using Finance.Api.Application.DTOs.Category;

namespace Finance.Api.Application.DTOs.Expenses;

public class FilteredAnalyticsDto
{
    public SummaryDto Summary { get; set; } = new();
    public List<CategorySpendDto> ByCategory { get; set; } = [];
    public DateDistributionDto DateDistribution { get; set; } = new();
}

public class SummaryDto
{
    public decimal TotalSpent { get; set; }
    public int Count { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
}

public class DateDistributionDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysWithExpenses { get; set; }
}