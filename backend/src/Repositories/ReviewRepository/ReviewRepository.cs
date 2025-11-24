using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ReviewRepository;

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

    public async Task<bool> UserPostedReviewForProduct(ObjectId userId, ObjectId productId,
        IClientSessionHandle? session = null)
    {
        if (session is not null)
        {
            return await _reviews
                .Find(session, r => r.UserId == userId && r.ProductId == productId)
                .AnyAsync();
        }

        return await _reviews
            .Find(r => r.UserId == userId && r.ProductId == productId)
            .AnyAsync();
    }

    public async Task<Review?> GetReviewById(ObjectId id, IClientSessionHandle? session = null)
    {
        if (session is not null)
        {
            return await _reviews.Find(session, r => r.Id == id).FirstOrDefaultAsync();
        }

        return await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateReview(Review review, IClientSessionHandle? session = null)
    {
        if (session is not null)
        {
            await _reviews.InsertOneAsync(session, review);
            return;
        }

        await _reviews.InsertOneAsync(review);
    }

    public async Task UpdateReview(Review review)
    {
        await _reviews.ReplaceOneAsync(r => r.Id == review.Id, review);
    }

    public async Task DeleteReview(ObjectId id, IClientSessionHandle? session = null)
    {
        if (session is not null)
        {
            await _reviews.DeleteOneAsync(session, r => r.Id == id);
            return;
        }

        await _reviews.DeleteOneAsync(r => r.Id == id);
    }
}