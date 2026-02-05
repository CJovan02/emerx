using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.OrderItems.Request;

public sealed record OrderItemRequest
{
    [Required] public required string ProductId { get; init; }
    [Required] public required int Quantity { get; init; }
}