using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Products.Request;

public sealed record ProductRequest
{
    [Required] public required string Name { get; init; }
    [Required] public required string Category { get; init; }
    [Required] public required string Image { get; init; }
    [Required] public required decimal Price { get; init; }
}