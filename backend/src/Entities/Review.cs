using MongoDB.Bson;

namespace EMerx.Entities;

public class Review : BaseEntity
{
    public required ObjectId UserId { get; init; }

    public required string UserFullName { get; init; }

    public required ObjectId ProductId { get; init; }

    public required double Rating { get; init; }

    public string? Description { get; init; }
}