using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Reviews;

public static class ReviewsExtensions
{
    public static ReviewResponse ToResponse(this Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            ProductId = review.ProductId,
            Rating = review.Rating,
            Description = review.Description
        };
    }

    public static Review ToDomain(this ReviewRequest request)
    {
        return new Review
        {
            Id = new ObjectId(),
            UserId = new ObjectId(request.UserId),
            ProductId = new ObjectId(request.ProductId),
            Rating = request.Rating,
            Description = request.Description,
        };
    }
}