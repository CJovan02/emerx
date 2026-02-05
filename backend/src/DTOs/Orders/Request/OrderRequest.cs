using System.ComponentModel.DataAnnotations;
using EMerx.DTOs.OrderItems.Request;
using EMerx.Entities;

namespace EMerx.DTOs.Orders.Request;

public sealed record OrderRequest
{
    [Required] public required string UserId { get; init; }
    [Required] public required List<OrderItemRequest> Items { get; init; }
    [Required] public required Address Address { get; init; }
}