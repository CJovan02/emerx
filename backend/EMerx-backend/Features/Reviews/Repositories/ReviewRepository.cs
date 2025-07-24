using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Features.Reviews.Repositories;

public class ReviewRepository(MongoDbContext context) : IReviewRepository
{
    private IMongoCollection<Review> _reviews = context.Reviews;

    public async Task<IEnumerable<Review>> GetReviews()
    {
        return await _reviews.Find(r => true).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsForProduct(ObjectId productId)
    {
        return await _reviews.Find(r => r.ProductId == productId).ToListAsync();
    }

    public async Task<Review> GetReviewById(ObjectId id)
    {
        return await _reviews.Find(r => r.ProductId == id).FirstOrDefaultAsync();
    }

    public async Task CreateReview(Review review)
    {
        await _reviews.InsertOneAsync(review);
    }

    public async Task UpdateReview(Review review)
    {
        await _reviews.ReplaceOneAsync(r => r.ProductId == review.ProductId, review);
    }

    public async Task DeleteReview(ObjectId id)
    {
        await _reviews.DeleteOneAsync(r => r.ProductId == id);
    }
}