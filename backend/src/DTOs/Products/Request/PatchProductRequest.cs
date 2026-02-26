using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Products.Request;

public sealed record PatchProductRequest
{
    public string? Name { get; init; }
    public string? Category { get; init; }
    public IFormFile? Image { get; init; }

    // Since this is a patch method, every field is optional.
    // If the image field is null, server doesn't know if th1`e image should be deleted or left untouched.
    // We use this prop to determine if we should delete the image or not.
    [Required] public required bool DeleteImageOnly { get; init; }
    public decimal? Price { get; init; }
}