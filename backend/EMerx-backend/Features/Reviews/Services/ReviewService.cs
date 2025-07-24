using EMerx_backend.Features.Products.Errors;
using EMerx_backend.Features.Products.Repositories;
using EMerx_backend.Features.Reviews.Dtos;
using EMerx_backend.Features.Reviews.Errors;
using EMerx_backend.Features.Reviews.Repositories;
using EMerx_backend.Features.Users.Errors;
using EMerx_backend.Features.Users.Repositories;
using FluentResults;
using Mapster;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository  _productRepository;
    private readonly IUserRepository _userRepository;

    public ReviewService(IReviewRepository reviewRepository,  IProductRepository productRepository,  IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IEnumerable<ReviewDto>>> GetReviews()
    {
        return Result.Ok((await _reviewRepository.GetReviews())
            .Adapt<IEnumerable<ReviewDto>>());
    }

    public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByProductId(ObjectId id)
    {
        if (await _productRepository.GetProductById(id) is null)
            return Result.Fail<IEnumerable<ReviewDto>>(new ProductNotFoundError(id));
        
        return Result.Ok((
            (await _reviewRepository.GetReviewsForProduct(id))
            .Adapt<IEnumerable<ReviewDto>>()));
    }

    public async Task<Result<ReviewDto>> GetReviewById(ObjectId id)
    {
        var review  = await _reviewRepository.GetReviewById(id);
        if (review is null)
            return Result.Fail<ReviewDto>(new ReviewNotFoundError(id));
        return Result.Ok(review.Adapt<ReviewDto>());
    }

    public async Task<Result<ReviewDto>> CreateReview(CreateReviewDto reviewDto)
    {
        if(await _productRepository.GetProductById(reviewDto.ProductId) is null)
            return Result.Fail<ReviewDto>(new ProductNotFoundError(reviewDto.ProductId));
        if (await _userRepository.GetUserById(reviewDto.UserId) is null)
            return Result.Fail<ReviewDto>(new UserNotFoundError(reviewDto.UserId));
        
        var review =  reviewDto.Adapt<Review>();
        await _reviewRepository.CreateReview(review);
        return Result.Ok(review.Adapt<ReviewDto>());
    }

    public async Task<Result<ReviewDto>> UpdateReview(ReviewDto reviewDto)
    {
        var review = await _reviewRepository.GetReviewById(reviewDto.Id);
        if (review is null)
            return Result.Fail<ReviewDto>(new ReviewNotFoundError(reviewDto.Id));
        
        review.Description =  reviewDto.Description;
        review.Rating = reviewDto.Rating;
        await _reviewRepository.UpdateReview(review);
        
        return Result.Ok(review.Adapt<ReviewDto>());
    }

    public async Task<Result> DeleteReview(ObjectId id)
    {
        if (await _reviewRepository.GetReviewById(id) is null)
            return Result.Fail(new  ReviewNotFoundError(id));
        await _reviewRepository.DeleteReview(id);
        return Result.Ok();
    }
}