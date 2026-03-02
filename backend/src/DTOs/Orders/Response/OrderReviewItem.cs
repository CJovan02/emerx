using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Orders.Response;

public record OrderReviewItem
{
    [Required] public required string ProductId { get; init; }
    [Required] public required string ProductName { get; init; }
    public string? ImageUrl { get; init; }
    [Required] public required decimal UnitPrice { get; init; }
    [Required] public required int Quantity { get; init; }
    [Required] public required decimal LineTotal { get; init; }
}