using EMerx.Entities;
using FluentValidation;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Request;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.UserId)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Please provide a valid user ID.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Please provide at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestValidator());

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator());
    }
}

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Please provide a valid product ID.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 20)
            .WithMessage("The quantity must be between 1 and 20.");
    }
}

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