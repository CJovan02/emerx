using EMerx.DTOs.OrderItems.Request;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Request;

public sealed record OrderRequest(
    string UserId,
    List<OrderItemRequest> Items,
    Address Address
);