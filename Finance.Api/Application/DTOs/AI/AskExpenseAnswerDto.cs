using Finance.Api.Application.DTOs.Expenses;

namespace Finance.Api.Application.DTOs.AI;

public sealed class AskExpenseAnswerDto
{
    public string Answer { get; set; } = string.Empty;

    public FilterExpenseDto FiltersUsed { get; set; } = new();

    public object? Analytics { get; set; }

    public object? Expenses { get; set; }
}
