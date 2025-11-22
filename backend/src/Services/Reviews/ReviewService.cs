using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;

namespace EMerx.Services.Reviews;

public class ReviewService(
    IReviewRepository reviewRepository,
    IUserRepository userRepository,
    IProductRepository productRepository) : IReviewService
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

    public async Task<Result<ReviewResponse>> CreateAsync(ReviewRequest request)
    {
        var userId = new ObjectId(request.UserId);
        var user = await userRepository.GetUserById(userId);
        if (user is null)
        {
            return Result<ReviewResponse>.Failure(UserErrors.NotFound(userId));
        }

        var productId = new ObjectId(request.ProductId);
        var product = await productRepository.GetProductById(productId);
        if (product is null)
        {
            return Result<ReviewResponse>.Failure(ProductErrors.NotFound(productId));
        }

        var review = request.ToDomain();

        // We also calculate the avg rating and increase the review count of the product
        var newReviewsCount = product.ReviewsCount + 1;
        var newAvgRating = (product.AverageRating * product.ReviewsCount + review.Rating) / newReviewsCount;
        await productRepository.UpdateProductReviewAsync(productId, newAvgRating, newReviewsCount);

        await reviewRepository.CreateReview(review);
        return Result<ReviewResponse>.Success(review.ToResponse());
    }

    public async Task<Result> DeleteAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var review = await reviewRepository.GetReviewById(objectId);

        if (review is null)
        {
            return Result.Failure(ReviewErrors.NotFound(objectId));
        }

        await reviewRepository.DeleteReview(review.Id);
        return Result.Success();
    }
}