using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews.Dtos;

public sealed record CreateReviewDto(
    ObjectId UserId,
    ObjectId ProductId,
    double Rating,
    string Description);