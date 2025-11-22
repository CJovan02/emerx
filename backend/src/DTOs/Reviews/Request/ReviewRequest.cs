using MongoDB.Bson;

namespace EMerx.DTOs.Reviews.Request;

public sealed record ReviewRequest(
    string UserId,
    string ProductId,
    double Rating,
    string Description);