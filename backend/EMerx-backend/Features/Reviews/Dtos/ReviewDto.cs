using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews.Dtos;

public sealed record ReviewDto(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    double Rating,
    string Description);