namespace Finance.Api.Application.DTOs.Expenses;

public class PaginationRequestDto
{ 
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}