using EMerx_backend.Entities;
using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Repositories.ReviewRepository;

public class ReviewRepository(MongoDbContext context) : IReviewRepository
{
    private IMongoCollection<Review> _reviews = context.Reviews;

    public async Task<IEnumerable<Review>> GetReviews()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Review>> GetReviewsForProduct(ObjectId productId)
    {
        throw new NotImplementedException();
    }

    public async Task<Review> GetReviewById(ObjectId id)
    {
        throw new NotImplementedException();
    }

    public async Task CreateReview(Review review)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateReview(Review review)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteReview(ObjectId id)
    {
        throw new NotImplementedException();
    }
}