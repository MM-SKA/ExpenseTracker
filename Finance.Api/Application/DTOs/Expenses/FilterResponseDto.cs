using Finance.Api.Application.DTOs.V1.Expenses;

namespace Finance.Api.Application.DTOs.Expenses;

public class FilterResponseDto
{
    public IReadOnlyCollection<ExpenseDtoV1> Expenses { get; set; } = [];
    public FilteredAnalyticsDto? Analytics { get; set; } // Null if not requested
}
