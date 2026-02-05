using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Reviews.Request;

public sealed record ReviewRequest
{
    [Required] public required string UserId { get; init; }
    [Required] public required string ProductId { get; init; }
    [Required] public required double Rating { get; init; }
    [Required] public required string Description { get; init; }
}