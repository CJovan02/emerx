using MongoDB.Bson;

namespace EMerx.ResultPattern.Errors;

public static class ReviewErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"Review with id {id} not found.");

    public static Error NotOrdered(ObjectId userId, ObjectId productId) =>
        new(StatusCodes.BadRequest, $"User {userId} must order product {productId} in order to leave a review");
    
    public static Error UserPostedReviewForProduct(ObjectId userId, ObjectId productId) =>
        new(StatusCodes.BadRequest, $"User {userId} can only post one review on product {productId}");
}