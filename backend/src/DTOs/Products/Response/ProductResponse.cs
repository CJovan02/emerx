using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace EMerx.DTOs.Products.Response;

public sealed record ProductResponse
{
    [Required] public required ObjectId Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required string Category { get; init; }
    public string? Image { get; init; }
    [Required] public required decimal Price { get; init; }
}