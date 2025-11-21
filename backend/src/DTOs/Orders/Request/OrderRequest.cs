using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Request;

public sealed record OrderRequest(
    string UserId,
    string ProductId,
    Address Address,
    int Quantity);