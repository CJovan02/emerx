using FluentValidation;

namespace EMerx.DTOs.Users.Request;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(3, 20)
            .WithMessage("Name must be between 5 and 20 characters long");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Surname is required")
            .Length(3, 20)
            .WithMessage("Surname must be between 5 and 30 characters long");
    }
}