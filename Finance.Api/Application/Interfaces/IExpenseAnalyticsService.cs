using Finance.Api.DTOs.Expenses;
using Finance.Api.Models;

public interface IExpenseAnalyticsService
{
    Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(
        int userId,
        FilterExpenseDto filters);
    
    Task<FilteredAnalyticsDto> CalculateAnalyticsAsync(
        List<Expense> filteredExpenses);
}