using EMerx.Utils;
using FluentValidation;

namespace EMerx.DTOs.Users.Request;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(5, 20)
            .WithMessage("Name must be between 5 and 20 characters long");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Surname is required")
            .Length(5, 20)
            .WithMessage("Surname must be between 5 and 30 characters long");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(30)
            .WithMessage("Password must be no more than 30 characters long")
            .Must(PasswordUtils.HasLowercase)
            .WithMessage("Password must have at least one lowercase character.")
            .Must(PasswordUtils.HasUppercase)
            .WithMessage("Password must have at least one uppercase character.");
    }
}