using EMerx.DTOs.OrderItems.Request;
using FluentValidation;

namespace EMerx.DTOs.Orders.Request;

public class OrderReviewRequestValidator : AbstractValidator<OrderReviewRequest>
{
    public OrderReviewRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Please provide at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestValidator());
    }
}