using MongoDB.Bson;

namespace EMerx.DTOs.Reviews.Request;

public sealed record ReviewRequest(
    ObjectId Id,
    ObjectId UserId,
    ObjectId ProductId,
    double Rating,
    string Description);