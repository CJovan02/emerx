using MongoDB.Bson;

namespace EMerx.Entities;

public class OrderItem
{
    public required ObjectId ProductId { get; init; }

    public required string Name { get; init; }

    public required decimal PriceAtOrder { get; init; }

    public required int Quantity { get; init; }
}