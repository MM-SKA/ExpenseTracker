using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.Validation;

public sealed class DateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is DateTime date)
        {
            if (date.Date > DateTime.UtcNow.Date)
            {
                return new ValidationResult("Future Date is not allowed !");
            }
        }
        return ValidationResult.Success;
    }
}