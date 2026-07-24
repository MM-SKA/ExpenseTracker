using Finance.Api.Application.DTOs.Auth;

using FluentValidation;

namespace Finance.Api.Application.Validation.User;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        _ = RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        _ = RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Length(10)
            .WithMessage("Phone number should be exactly 10 digits.")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number should contain only digits.");

        _ = RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]")
            .WithMessage("Password must contain at least one special character.");
    }
}
