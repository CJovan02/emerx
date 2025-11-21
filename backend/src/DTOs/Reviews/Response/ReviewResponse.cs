using MongoDB.Bson;

namespace EMerx.DTOs.Reviews.Response;

public sealed record ReviewResponse(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    double Rating,
    string Description);