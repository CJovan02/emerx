using FluentValidation;
using MongoDB.Bson;

namespace EMerx.DTOs.OrderItems.Request;

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