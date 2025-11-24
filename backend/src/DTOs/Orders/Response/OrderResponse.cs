using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Response;

public sealed record OrderResponse(
    ObjectId Id,
    ObjectId UserId,
    List<OrderItem> Items,
    Address Address,
    decimal Price,
    DateTime PlacedAt
);