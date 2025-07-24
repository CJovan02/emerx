using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviews();
    Task<IEnumerable<Review>> GetReviewsForProduct(ObjectId productId);
    Task<Review?> GetReviewById(ObjectId id);
    Task CreateReview(Review review);
    Task UpdateReview(Review review);
    Task DeleteReview(ObjectId id);
}