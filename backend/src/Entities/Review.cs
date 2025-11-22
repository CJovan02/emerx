using MongoDB.Bson;

namespace EMerx.Entities;

public class Review : BaseEntity
{
    public ObjectId UserId { get; init; }

    public ObjectId ProductId { get; init; }

    public double Rating { get; init; }

    public string? Description { get; init; }
}