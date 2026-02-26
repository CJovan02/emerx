using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Products.Request;

public sealed record CreateProductRequest
{
    [Required] public required string Name { get; init; }
    [Required] public required string Category { get; init; }
    public IFormFile? Image { get; init; }
    [Required] public required decimal Price { get; init; }
}