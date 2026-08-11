using Finance.Api.Application.DTOs.V1.Expenses;

namespace Finance.Api.Application.DTOs.Expenses;

public class FilteredPagedExpenseResponseDto
{
    public IReadOnlyCollection<ExpenseDtoV1> Items { get; set; } = [];

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public FilteredAnalyticsDto? Analytics { get; set; }
}
