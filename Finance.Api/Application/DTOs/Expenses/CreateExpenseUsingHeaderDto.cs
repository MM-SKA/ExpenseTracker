namespace Finance.Api.Application.DTOs.Expenses;

public class CreateExpenseUsingHeaderDto
{
    public required string Desc { get; set; }
    public required decimal Amount { get; set; }
}