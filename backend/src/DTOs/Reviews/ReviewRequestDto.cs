using MongoDB.Bson;

namespace EMerx.DTOs.Reviews;

public sealed record ReviewRequestDto(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    double Rating,
    string Description);