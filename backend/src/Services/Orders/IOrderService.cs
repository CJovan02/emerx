using EMerx.DTOs.Id;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Orders;

public interface IOrderService
{
    //Order Response Currently only gets id that can be changed
    Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync();

    Task<Result<OrderResponse>> GetOrderAsync(IdRequest request);

    Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request);

    Task<Result> DeleteOrderAsync(IdRequest request);
}