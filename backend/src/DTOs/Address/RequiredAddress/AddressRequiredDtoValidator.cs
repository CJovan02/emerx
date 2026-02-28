using FluentValidation;

namespace EMerx.DTOs.Address;

public class AddressRequiredDtoValidator : AbstractValidator<AddressRequiredDto>
{
    public AddressRequiredDtoValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .Length(3, 50)
            .WithMessage("City must be between 3 and 50 characters");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .Length(3, 50)
            .WithMessage("Street must be between 3 and 50 characters");

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .WithMessage("House Number is required")
            .Length(3, 50)
            .WithMessage("House Number must be between 3 and 50 characters");
    }
}