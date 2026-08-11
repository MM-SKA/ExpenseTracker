using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Expenses;

public class FilterExpenseDto
{
    public string? CategoryId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public string? Notes { get; set; }

    public bool IncludeAnalytics { get; set; }

    public string? Location { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be atleast 1")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    public string SortOrder { get; set; } = "recent";
}
