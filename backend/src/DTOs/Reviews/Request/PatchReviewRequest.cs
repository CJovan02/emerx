namespace EMerx.DTOs.Reviews.Request;

public sealed record PatchReviewRequest
{
    public double? Rating { get; init; }
    public string? Description { get; init; }
}
