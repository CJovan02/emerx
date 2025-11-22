using EMerx.DTOs.Id;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Orders;

public interface IOrderService
{
    //Order Response Currently only gets id that can be changed
    Task<Result<IEnumerable<OrderResponse>>> GetAllAsync();

    Task<Result<OrderResponse>> GetByIdAsync(IdRequest request);

    Task<Result<OrderResponse>> CreateAsync(OrderRequest request);

    Task<Result> DeleteAsync(IdRequest request);
}