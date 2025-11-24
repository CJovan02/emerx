using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Reviews;

public interface IReviewService
{
    Task<Result<IEnumerable<ReviewResponse>>> GetAllAsync();
    
    Task<Result<ReviewResponse>> GetByIdAsync(IdRequest request);

    Task<Result<IEnumerable<ReviewResponse>>> GetByProductIdAsync(IdRequest request);
    
    Task<Result<ReviewResponse>> CreateAsync(ReviewRequest request);
    
    Task<Result> DeleteAsync(IdRequest request);
}