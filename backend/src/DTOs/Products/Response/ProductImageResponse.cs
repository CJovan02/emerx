using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Products.Response;

public sealed record ProductImageResponse
{
    [Required] public required string PublicId { get; init; }
    [Required] public required string Url { get; init; }
    [Required] public required bool IsThumbnail { get; init; }
    [Required] public required int Order { get; init; }
}