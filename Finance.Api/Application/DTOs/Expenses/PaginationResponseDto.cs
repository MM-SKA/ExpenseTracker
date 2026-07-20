namespace Finance.Api.Application.DTOs.Expenses;

public class PaginationResponseDto<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}
