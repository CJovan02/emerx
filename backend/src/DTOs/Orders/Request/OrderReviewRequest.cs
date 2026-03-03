using System.ComponentModel.DataAnnotations;
using EMerx.DTOs.OrderItems.Request;

namespace EMerx.DTOs.Orders.Request;

public record OrderReviewRequest
{
    [Required] public required List<OrderItemRequest> Items { get; init; }

}