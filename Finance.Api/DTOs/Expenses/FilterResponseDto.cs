public class FilterResponseDto
{
    public List<ExpenseDto> Expenses { get; set; }
    public FilteredAnalyticsDto? Analytics { get; set; } // Null if not requested
}