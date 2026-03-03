using System.ComponentModel.DataAnnotations;
using EMerx.DTOs.Address;
using EMerx.DTOs.OrderItems.Request;

namespace EMerx.DTOs.Orders.Request;

public sealed record OrderRequest
{
    [Required] public required List<OrderItemRequest> Items { get; init; }
    [Required] public required AddressRequiredDto Address { get; init; }
}