using EMerx.DTOs.Id;
using EMerx.DTOs.Orders;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.Infrastructure.MongoDb;
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
    IProductRepository productRepository,
    MongoDbContext mongoDbContext) : IOrderService
{
    public async Task<Result<IEnumerable<OrderResponse>>> GetAllAsync()
    {
        return (await orderRepository.GetOrders())
            .Select(order => order.ToResponse())
            .ToList();
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var order = await orderRepository.GetOrderById(objectId);

        if (order is null)
        {
            return Result<OrderResponse>.Failure(OrderErrors.NotFound(objectId));
        }

        return Result<OrderResponse>.Success(order.ToResponse());
    }

    public async Task<Result<OrderResponse>> CreateAsync(OrderRequest request)
    {
        using var session = await mongoDbContext.StartSessionAsync();

        try
        {
            session.StartTransaction();

            var userId = ObjectId.Parse(request.UserId);
            var user = await userRepository.GetUserById(userId, session);
            if (user is null)
            {
                await session.AbortTransactionAsync();
                return Result<OrderResponse>.Failure(UserErrors.NotFound(userId));
            }

            // We normalize ordered items, we sum up the quantities of items that have the same id
            // If some bug on frontend happens and duplicate items get sent
            var normalizedItems = request.Items
                .GroupBy(x => x.ProductId)
                .Select(g => new OrderItemRequest(
                    ProductId: g.Key,
                    Quantity: g.Sum(x => x.Quantity))
                ).ToList();
            var normalizedRequest = request with { Items = normalizedItems };

            // We need to fetch all the products ordered, to see if all the requested ids are correct and
            // to check the price and the name of the products.
            // We could send the price and the name from the frontend inside a request to save on the database reads but
            // that is a security hole, price needs to be read from the database and not specified.

            var productIds = normalizedItems.Select(x => ObjectId.Parse(x.ProductId)).ToList();
            var products = (await productRepository.GetProductsByIds(productIds, session)).ToList();

            var missingProducts = productIds.Except(products.Select(x => x.Id)).ToList();
            if (missingProducts.Any())
            {
                await session.AbortTransactionAsync();
                return Result<OrderResponse>.Failure(OrderErrors.NotFound(missingProducts));
            }

            var order = normalizedRequest.ToDomain(products);
            await orderRepository.CreateOrder(order, session);

            await session.CommitTransactionAsync();
            return Result<OrderResponse>.Success(order.ToResponse());
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task<Result> DeleteAsync(IdRequest request)
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