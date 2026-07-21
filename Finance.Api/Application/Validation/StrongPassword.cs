using Finance.Api.Application.DTOs.Auth;

using FluentValidation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        _ = RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(8)
        .Matches("[A-Z]")
        .WithMessage("PASSWORD MUST CONTAIN ATLEAST ONE UPPERCASE LETTER")
        .Matches("[a-z]")
        .WithMessage("password must contain a lowercase letter.")
        .Matches("[0-9]")
        .WithMessage("Password must contain a digit.")
        .Matches(@"[\W_]")
        .WithMessage("Password must contain a special character.");

    }
}