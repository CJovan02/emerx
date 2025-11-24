using EMerx.Entities;
using FluentValidation;

namespace EMerx.DTOs.Addresses;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("Please provide a city.")
            .Length(3, 30)
            .WithMessage("The city must be between 3 and 30 characters.");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Please provide a street.")
            .Length(3, 30)
            .WithMessage("The street must be between 3 and 30 characters.");

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .WithMessage("Please provide a house number.")
            .Length(3, 30)
            .WithMessage("The house number must be between 3 and 30 characters.");
    }
}