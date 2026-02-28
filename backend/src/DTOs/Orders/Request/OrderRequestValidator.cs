using EMerx.DTOs.Address;
using EMerx.DTOs.OrderItems.Request;
using FluentValidation;

namespace EMerx.DTOs.Orders.Request;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Please provide at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestValidator());

        RuleFor(x => x.Address)
            .SetValidator(new AddressRequiredDtoValidator());
    }
}