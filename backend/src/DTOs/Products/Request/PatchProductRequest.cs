using System.ComponentModel.DataAnnotations;
using EMerx.Common;

namespace EMerx.DTOs.Products.Request;

public sealed record PatchProductRequest
{
    public string? Name { get; init; }
    public string? Category { get; init; }
    [Required] public required OptionalField<IFormFile?> Image { get; init; }
    public decimal? Price { get; init; }
}