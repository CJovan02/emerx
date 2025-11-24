using MongoDB.Bson;

namespace EMerx.Entities;

public class Order : BaseEntity
{
    // TODO Discuss order in more detail.
    // - User should be able to order multiple products, not just one
    // - How to store ordered products?
    //  - Suggestion: items: [ { ProductId: xxxx, PriceAtOrder: xxxx, Quantity: xxxx}, ... ]
    //  - We don't store entire product object inside items array, only some basic info so we don't have to query
    //    all of the items. If the item gets deleted, we still have the last snapshot of it during the order time
    public required ObjectId UserId { get; init; }

    public required List<OrderItem> Items { get; init; }

    public required Address Address { get; init; }

    public required decimal Price { get; init; }

    public DateTime PlacedAt { get; init; } = DateTime.UtcNow;
}

public class OrderItem
{
    public required ObjectId ProductId { get; init; }

    public required string Name { get; init; }

    public required decimal PriceAtOrder { get; init; }

    public required int Quantity { get; init; }
}