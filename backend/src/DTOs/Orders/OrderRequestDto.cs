using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders;

public sealed record OrderRequestDto(
    ObjectId UserId,
    ObjectId ProductId,
    Address Address,
    int Quantity);