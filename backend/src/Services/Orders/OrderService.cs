using EMerx.DTOs.Id;
using EMerx.DTOs.Orders;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;

namespace EMerx.Services.Orders;

public class OrderService(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository) : IOrderService
{
    public async Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync()
    {
        return (await orderRepository.GetOrders())
            .Select(order => order.ToResponse())
            .ToList();
    }

    public async Task<Result<OrderResponse>> GetOrderAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var order = await orderRepository.GetOrderById(objectId);

        if (order is null)
        {
            return Result<OrderResponse>.Failure(OrderErrors.NotFound(objectId));
        }

        return Result<OrderResponse>.Success(order.ToResponse());
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request)
    {
        var userId = ObjectId.Parse(request.UserId);
        var user = await userRepository.GetUserById(userId);
        if (user is null)
        {
            return Result<OrderResponse>.Failure(UserErrors.NotFound(userId));
        }

        var productId = ObjectId.Parse(request.ProductId);
        var product = await productRepository.GetProductById(productId);
        if (product is null)
        {
            return Result<OrderResponse>.Failure(ProductErrors.NotFound(productId));
        }

        var order = request.ToDomain();
        await orderRepository.CreateOrder(order);
        return Result<OrderResponse>.Success(order.ToResponse());
    }

    public async Task<Result> DeleteOrderAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var order = await orderRepository.GetOrderById(objectId);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(objectId));
        }

        await orderRepository.DeleteOrder(order.Id);
        return Result.Success();
    }
}