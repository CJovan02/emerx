using MongoDB.Bson;

namespace EMerx.Entities;

public class Order : BaseEntity
{
    public required ObjectId UserId { get; init; }

    public required List<OrderItem> Items { get; init; }

    public required Address Address { get; init; }

    public required decimal Price { get; init; }

    public DateTime PlacedAt { get; init; } = DateTime.UtcNow;
}