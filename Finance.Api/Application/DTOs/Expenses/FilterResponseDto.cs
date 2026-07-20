namespace Finance.Api.Application.DTOs.Expenses;

public class FilterResponseDto
{
    public IReadOnlyCollection<ExpenseDto> Expenses { get; set; } = [];
    public FilteredAnalyticsDto? Analytics { get; set; } // Null if not requested
}