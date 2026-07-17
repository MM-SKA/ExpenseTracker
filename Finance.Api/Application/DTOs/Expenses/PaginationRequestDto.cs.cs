using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Expenses;

public class PaginationRequestDto
{

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be atleast 1")]

    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
}
