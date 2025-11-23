using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.Entities;
using EMerx.Repositories.OrderRepository;
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
    IProductRepository productRepository,
    IOrderRepository orderRepository) : IReviewService
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

    // TODO create a transaction instead of 6 separate database calls
    public async Task<Result<ReviewResponse>> CreateAsync(ReviewRequest request)
    {
        // Discuss user and product checks, maybe they are not required
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

        // We check if user ordered this item, if not then he can't leave review
        if (!(await orderRepository.HasUserOrderedProduct(userId, productId)))
        {
            return Result<ReviewResponse>.Failure(ReviewErrors.NotOrdered(userId, productId));
        }
        
        // Also, if user already left the review then he can't create another one
        if (await reviewRepository.UserPostedReviewForProduct(userId, productId))
        {
            return Result<ReviewResponse>.Failure(ReviewErrors.UserPostedReviewForProduct(userId, productId));
        }

        var review = request.ToDomain();

        // We also calculate the avg rating and increase the review count of the product
        await CalculateAndIncreaseProductRating(product, review.Rating);

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

        // We also calculate the avg rating of the product
        var product = await productRepository.GetProductById(review.ProductId);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(review.ProductId));
        }
        await CalculateAndDecreaseProductRating(product, review.Rating);

        return Result.Success();
    }

    private async Task CalculateAndIncreaseProductRating(Product product, double rating)
    {
        var newReviewsCount = product.ReviewsCount + 1;
        var newSumRatings = product.SumRatings + rating;
        var newAvgRating = newSumRatings / newReviewsCount;
        await productRepository.UpdateProductReviewAsync(product.Id, newAvgRating, newSumRatings, newReviewsCount);
    }
    
    private async Task CalculateAndDecreaseProductRating(Product product, double rating)
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
        await productRepository.UpdateProductReviewAsync(product.Id, newAvgRating, newSumRatings, newReviewsCount);
    }}