using EMerx.DTOs.Addresses;
using FluentValidation;

namespace EMerx.DTOs.Users.Request;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(3, 20)
            .WithMessage("Name must be between 3 and 20 characters long");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Surname is required")
            .Length(3, 20)
            .WithMessage("Surname must be between 3 and 20 characters long");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!)
                .SetValidator(new AddressValidator());
        });
    }
}
