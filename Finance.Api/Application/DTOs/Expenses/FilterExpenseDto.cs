using Finance.Api.DTOs.Expenses;

public class FilterExpenseDto{
    public int? categoryId {get;set;}

    public DateTime? startDate {get;set;}

    public DateTime? endDate {get;set;}

    public decimal? minAmount {get;set;}

    public decimal? maxAmount {get;set;}

    public int? month {get;set;}

    public int? year {get;set;}

    public string? notes {get;set;}

    public bool includeAnalytics {get;set;} = false;
}