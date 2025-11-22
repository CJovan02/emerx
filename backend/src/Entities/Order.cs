using MongoDB.Bson;

namespace EMerx.Entities;

public class Order : BaseEntity
{
    public ObjectId UserId { get; set; }

    public ObjectId ProductId { get; set; }

    public required Address Address { get; set; }

    public int Quantity { get; set; }

    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
}