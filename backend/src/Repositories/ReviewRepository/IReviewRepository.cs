using EMerx.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ReviewRepository;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviews();
    Task<IEnumerable<Review>> GetReviewsForProduct(ObjectId productId);
    Task<bool> UserPostedReviewForProduct(ObjectId userId, ObjectId productId, IClientSessionHandle? session = null);
    Task<Review?> GetReviewById(ObjectId id, IClientSessionHandle? session = null);
    Task CreateReview(Review review, IClientSessionHandle? session = null);
    Task UpdateReview(Review review);
    Task DeleteReview(ObjectId id, IClientSessionHandle? session = null);
}