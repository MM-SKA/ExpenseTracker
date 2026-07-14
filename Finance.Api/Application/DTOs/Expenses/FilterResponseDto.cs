namespace Finance.Api.Application.DTOs.Expenses;

public class FilterResponseDto
{
    public List<ExpenseDto> Expenses { get; set; } = new();
    public FilteredAnalyticsDto? Analytics { get; set; } // Null if not requested
}