using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Orders.Response;

public record OrderReviewResponse
{
    [Required] public required List<OrderReviewItem> Items { get; init; }
    [Required] public required decimal Total { get; init; }
}