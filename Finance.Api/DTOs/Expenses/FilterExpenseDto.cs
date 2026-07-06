using Finance.Api.DTOs.Expenses;

public class FilterExpenseDto{
    public int? categoryId {get;set;}

    public DateTime? StartDate {get;set;}

    public DateTime? EndDate {get;set;}

    public decimal? MinAmount {get;set;}

    public decimal? MaxAmount {get;set;}

    public int? Month {get;set;}

    public int? Year {get;set;}

    public string? Notes {get;set;}
}