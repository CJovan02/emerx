using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Reviews.Response;

public sealed record ReviewResponse
{
    [Required] public required string Id { get; init; }
    [Required] public required string UserId { get; init; }

    [Required] public required string UserFullName { get; init; }
    [Required] public required string ProductId { get; init; }
    [Required] public required double Rating { get; init; }
    [Required] public required string Description { get; init; }
}