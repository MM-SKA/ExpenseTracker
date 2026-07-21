using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Office2016.Drawing.Command;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Finance.Api.Application.Validation;

public sealed partial class StrongPasswordAttribute : ValidationAttribute
{
    [GeneratedRegex("[A-Z]")]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowerCaseRegex();

    [GeneratedRegex("[0-9]")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"\W_")]
    private static partial Regex SpecialCharacterRegex();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
        {
            return new ValidationResult("Password must not be empty");
        }

        if (!UpperCaseRegex().IsMatch(password))
        {
            return new ValidationResult("PASSWORD MUST CONTAIN ATLEAST ONE UPPERCASE LETTER");
        }

        if (!LowerCaseRegex().IsMatch(password))
        {
            return new ValidationResult("password must contain atleast one lowercase letter");
        }

        if (!DigitRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain atleast one Digit");
        }

        if (!SpecialCharacterRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain atleast one $pecial Ch@racter");
        }

        return ValidationResult.Success;
    }
}
