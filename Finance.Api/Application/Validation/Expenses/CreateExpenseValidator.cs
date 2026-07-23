using System.ComponentModel.DataAnnotations;
using Finance.Api.Application.DTOs.Expenses;
using FluentValidation;

namespace Finance.Api.Application.Validation;

public sealed class CreateExpenseValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseValidator()
    {
        _ = RuleFor(x => x.Date)
        .LessThanOrEqualTo(DateTime.UtcNow.Date)
        .WithMessage("Future Dates are NOT ALLOWED !");

        _ = RuleFor(x => x.Amount)
        .GreaterThan(0)
        .WithMessage("Amount can not be less than or equal to Zero (0)");
    }
}
