using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Response;

public sealed record OrderResponse(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    Address Address,
    int Quantity);