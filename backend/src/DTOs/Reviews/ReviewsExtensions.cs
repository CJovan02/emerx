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
            Id = review.Id.ToString(),
            UserId = review.UserId.ToString(),
            ProductId = review.ProductId.ToString(),
            UserFullName = review.UserFullName,
            Rating = review.Rating,
            Description = review.Description
        };
    }

    public static Review ToDomain(this ReviewRequest request, ObjectId userId, string userFullName)
    {
        return new Review
        {
            UserId = userId,
            UserFullName = userFullName,
            ProductId = new ObjectId(request.ProductId),
            Rating = request.Rating,
            Description = request.Description,
        };
    }
}