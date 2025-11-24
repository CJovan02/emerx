using EMerx.DTOs.Addresses;
using EMerx.DTOs.OrderItems.Request;
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

