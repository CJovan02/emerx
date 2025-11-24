using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Services.Reviews;

public class ReviewService(
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    MongoDbContext mongoDbContext) : IReviewService
{
    public async Task<Result<IEnumerable<ReviewResponse>>> GetAllAsync()
    {
        return (await reviewRepository.GetReviews()).Select(review => review.ToResponse()).ToList();
    }

    public async Task<Result<ReviewResponse>> GetByIdAsync(IdRequest request)
    {
        var objectId = new ObjectId(request.Id);
        var review = await reviewRepository.GetReviewById(objectId);

        if (review is null)
        {
            return Result<ReviewResponse>.Failure(ReviewErrors.NotFound(objectId));
        }

        return Result<ReviewResponse>.Success(review.ToResponse());
    }

    public async Task<Result<IEnumerable<ReviewResponse>>> GetByProductIdAsync(IdRequest request)
    {
        var productId = new ObjectId(request.Id);
        var products = await reviewRepository.GetReviewsForProduct(productId);

        var productsResponse = products.Select(x => x.ToResponse()).ToList();

        return Result<IEnumerable<ReviewResponse>>.Success(productsResponse);
    }

    public async Task<Result<ReviewResponse>> CreateAsync(ReviewRequest request)
    {
        using var session = await mongoDbContext.StartSessionAsync();

        try
        {
            session.StartTransaction();
            var userId = new ObjectId(request.UserId);

            // Discuss user check it's required
            // var user = await userRepository.GetUserById(userId, session);
            // if (user is null)
            // {
            //     await session.AbortTransactionAsync();
            //     return Result<ReviewResponse>.Failure(UserErrors.NotFound(userId));
            // }

            var productId = new ObjectId(request.ProductId);
            var product = await productRepository.GetProductById(productId, session);
            if (product is null)
            {
                await session.AbortTransactionAsync();
                return Result<ReviewResponse>.Failure(ProductErrors.NotFound(productId));
            }

            // We check if user ordered this item, if not then he can't leave review
            if (!(await orderRepository.HasUserOrderedProduct(userId, productId, session)))
            {
                await session.AbortTransactionAsync();
                return Result<ReviewResponse>.Failure(ReviewErrors.NotOrdered(userId, productId));
            }

            // Also, if user already left the review then he can't create another one
            if (await reviewRepository.UserPostedReviewForProduct(userId, productId, session))
            {
                await session.AbortTransactionAsync();
                return Result<ReviewResponse>.Failure(ReviewErrors.UserPostedReviewForProduct(userId, productId));
            }

            var review = request.ToDomain();

            // We also calculate the avg rating and increase the review count of the product
            await CalculateAndIncreaseProductRating(product, review.Rating, session);

            await reviewRepository.CreateReview(review, session);
            
            await session.CommitTransactionAsync();
            return Result<ReviewResponse>.Success(review.ToResponse());
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task<Result> DeleteAsync(IdRequest request)
    {
        using var session = await mongoDbContext.StartSessionAsync();

        try
        {
            session.StartTransaction();
            
            var objectId = ObjectId.Parse(request.Id);
            var review = await reviewRepository.GetReviewById(objectId, session);

            if (review is null)
            {
                await session.AbortTransactionAsync();
                return Result.Failure(ReviewErrors.NotFound(objectId));
            }

            await reviewRepository.DeleteReview(review.Id, session);

            // We also calculate the avg rating of the product
            var product = await productRepository.GetProductById(review.ProductId, session);
            if (product is null)
            {
                await session.AbortTransactionAsync();
                return Result.Failure(ProductErrors.NotFound(review.ProductId));
            }

            await CalculateAndDecreaseProductRating(product, review.Rating, session);

            await session.CommitTransactionAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    private async Task CalculateAndIncreaseProductRating(Product product, double rating, IClientSessionHandle session)
    {
        var newReviewsCount = product.ReviewsCount + 1;
        var newSumRatings = product.SumRatings + rating;
        var newAvgRating = newSumRatings / newReviewsCount;
        await productRepository.UpdateProductReviewAsync(product.Id, newAvgRating, newSumRatings, newReviewsCount, session);
    }

    private async Task CalculateAndDecreaseProductRating(Product product, double rating, IClientSessionHandle session)
    {
        var newReviewsCount = product.ReviewsCount - 1;
        var newSumRatings = product.SumRatings - rating;
        double newAvgRating;
        if (newReviewsCount > 0)
        {
            newAvgRating = newSumRatings / newReviewsCount;
        }
        else
        {
            newSumRatings = 0;
            newAvgRating = 0;
        }

        await productRepository.UpdateProductReviewAsync(product.Id, newAvgRating, newSumRatings, newReviewsCount, session);
    }
}