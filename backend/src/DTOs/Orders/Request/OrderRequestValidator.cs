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

        RuleFor(x => x.ProductId)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Please provide a valid product ID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}