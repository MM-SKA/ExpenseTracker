using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Models;

namespace Finance.Api.Application.Interfaces;

public interface IExpenseAnalyticsService
{
    Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(
        int userId,
        FilterExpenseDto filters);
    
    Task<FilteredAnalyticsDto> CalculateAnalyticsAsync(
        List<Expense> filteredExpenses);
}