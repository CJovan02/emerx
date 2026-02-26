using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace EMerx.DTOs.Products.Response;

public sealed record ProductResponse
{
    [Required] public required string Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required string Category { get; init; }

    public string? ThumbnailUrl { get; init; }
    [Required] public required decimal Price { get; init; }
    [Required] public required double AverageRating { get; init; }
}