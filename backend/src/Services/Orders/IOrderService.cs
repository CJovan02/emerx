using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Orders;

public interface IOrderService
{
    //Order Response Currently only gets id that can be changed
    Task<Result<PageOfResponse<OrderResponse>>> GetAllAsync(int page, int pageSize);

    Task<Result<OrderResponse>> GetByIdAsync(IdRequest request);

    // Client sends the id of the products that he wants to order and server checks their prices, stock and calculates
    // the total price. Used in the final step of the checkout process to pull the latest prices and products
    Task<Result<OrderReviewResponse>> GetOrderReview(OrderReviewRequest request);

    Task<Result<OrderResponse>> CreateAsync(string userId, OrderRequest request);

    Task<Result> DeleteAsync(IdRequest request);
}