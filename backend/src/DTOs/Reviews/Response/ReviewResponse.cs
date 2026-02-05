using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace EMerx.DTOs.Reviews.Response;

public sealed record ReviewResponse
{
    [Required] public required ObjectId Id { get; init; }
    [Required] public required ObjectId UserId { get; init; }
    [Required] public required ObjectId ProductId { get; init; }
    [Required] public required double Rating { get; init; }
    [Required] public required string Description { get; init; }
}