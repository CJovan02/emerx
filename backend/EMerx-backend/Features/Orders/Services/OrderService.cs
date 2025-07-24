using EMerx_backend.Features.Orders.Dtos;
using EMerx_backend.Features.Orders.Errors;
using EMerx_backend.Features.Orders.Repositories;
using EMerx_backend.Features.Products.Errors;
using EMerx_backend.Features.Products.Repositories;
using EMerx_backend.Features.Users.Errors;
using EMerx_backend.Features.Users.Repositories;
using FluentResults;
using Mapster;
using MongoDB.Bson;

namespace EMerx_backend.Features.Orders.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetAllOrders()
    {
        return Result.Ok((await _orderRepository.GetOrders())
            .Adapt<IEnumerable<OrderDto>>());
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetAllOrdersForUser(ObjectId id)
    {
        if (await _userRepository.GetUserById(id) is null)
            return Result.Fail<IEnumerable<OrderDto>>(new UserNotFoundError(id));
        return Result.Ok((await _orderRepository.GetOrdersForUser(id))
            .Adapt<IEnumerable<OrderDto>>());
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetAllOrdersForProduct(ObjectId id)
    {
        if (await _productRepository.GetProductById(id) is null)
            return Result.Fail<IEnumerable<OrderDto>>(new ProductNotFoundError(id));
        return Result.Ok((await _orderRepository.GetOrdersForProduct(id))
            .Adapt<IEnumerable<OrderDto>>());
    }

    public async Task<Result<OrderDto>> GetOrderById(ObjectId id)
    {
        var order =  await _orderRepository.GetOrderById(id);
        if (order is null)
            return Result.Fail<OrderDto>(new OrderNotFoundError(id));
        return Result.Ok(order.Adapt<OrderDto>());
    }

    public async Task<Result<OrderDto>> CreateOrder(CreateOrderDto orderDto)
    {
        if (await _userRepository.GetUserById(orderDto.UserId) is null)
            return Result.Fail<OrderDto>(new UserNotFoundError(orderDto.UserId));
        if (await _productRepository.GetProductById(orderDto.ProductId) is null)
            return Result.Fail<OrderDto>(new ProductNotFoundError(orderDto.ProductId));
        
        var order = orderDto.Adapt<Order>();
        await _orderRepository.CreateOrder(order);
        
        return Result.Ok(order.Adapt<OrderDto>());
    }
    
    //should we even have update? 
    
    public async Task<Result> DeleteOrder(ObjectId id)
    {
        var order = await _orderRepository.GetOrderById(id);
        if (order is null)
            return Result.Fail(new OrderNotFoundError(id));
        await _orderRepository.DeleteOrder(order);
        return Result.Ok();
    }
}