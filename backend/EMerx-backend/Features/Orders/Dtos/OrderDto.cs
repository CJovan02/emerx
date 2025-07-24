using EMerx_backend.Shared;
using MongoDB.Bson;

namespace EMerx_backend.Features.Orders.Dtos;

public sealed record OrderDto(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    Address Address,
    int Quantity);