using FluentValidation;

namespace EMerx.DTOs.Users.Request;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .Length(5, 20)
            .WithMessage("Name must be between 5 and 20 characters long");

        RuleFor(x => x.Surname)
            .Length(5, 20)
            .WithMessage("Surname must be between 5 and 30 characters long");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address");

        // Password rules discuss
    }
}