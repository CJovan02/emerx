using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.OrderItems.Request;
using EMerx.DTOs.Orders;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Services.Orders;

public class OrderService(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository,
    ICloudinaryRepository cloudinaryRepository,
    MongoContext mongoContext) : IOrderService
{
    public async Task<Result<PageOfResponse<OrderResponse>>> GetAllAsync(int page, int pageSize)
    {
        var pageOfOrders = await orderRepository.GetOrders(page, pageSize);
        var productResponses = pageOfOrders
            .Items
            .Select(order => order.ToResponse())
            .ToList();

        var response = new PageOfResponse<OrderResponse>(
            productResponses,
            pageOfOrders.Page,
            pageOfOrders.PageSize,
            pageOfOrders.TotalItems);

        return Result<PageOfResponse<OrderResponse>>.Success(response);
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

    // Very similar implementation to the CreateAsync below, but now instead of creating an order and writing to db
    // we calculate the prices and return appropriate error responses.
    public async Task<Result<OrderReviewResponse>> GetOrderReview(OrderReviewRequest request)
    {
        var normalizedItems = NormalizeOrderItems(request.Items);
        var normalizedRequest = request with { Items = normalizedItems };

        var productIds = normalizedItems.Select(x => ObjectId.Parse(x.ProductId)).ToList();
        var products = (await productRepository.GetProductsByIds(productIds)).ToList();
        var productsDict = products.ToDictionary(x => x.Id);

        var missingProducts = GetMissingProducts(productIds, products).ToList();
        if (missingProducts.Any())
            return Result<OrderReviewResponse>.Failure(OrderErrors.NotFound(missingProducts));

        var violation = CheckProductsAvailability(normalizedItems, productsDict);
        if (violation is not null)
            return Result<OrderReviewResponse>.Failure(
                OrderErrors.QuantityNotAvailable(violation.Value.Item1, violation.Value.Item2, violation.Value.Item3));

        var returnResponse =
            normalizedRequest.ToResponse(productsDict, cloudinaryRepository.BuildProductThumbnailImageUrl);

        return Result<OrderReviewResponse>.Success(returnResponse);
    }

    public async Task<Result<OrderResponse>> CreateAsync(string userFirebaseId, OrderRequest request)
    {
        using var session = await mongoContext.StartSessionAsync();

        try
        {
            session.StartTransaction();

            var user = await userRepository.GetUserByFirebaseUid(userFirebaseId, session);
            if (user is null)
            {
                await session.AbortTransactionAsync();
                return Result<OrderResponse>.Failure(UserErrors.NotFound(userFirebaseId));
            }
            var userId = user.Id;

            var normalizedItems = NormalizeOrderItems(request.Items);
            var normalizedRequest = request with { Items = normalizedItems };

            var productIds = normalizedItems.Select(x => ObjectId.Parse(x.ProductId)).ToList();
            var products = (await productRepository.GetProductsByIds(productIds, session)).ToList();
            var productsDict = products.ToDictionary(x => x.Id);

            var missingProducts = GetMissingProducts(productIds, products).ToList();
            if (missingProducts.Any())
            {
                await session.AbortTransactionAsync();
                return Result<OrderResponse>.Failure(OrderErrors.NotFound(missingProducts));
            }

            var order = normalizedRequest.ToDomain(productsDict, userId, $"{user.Name} {user.Surname}");
            await orderRepository.CreateOrder(order, session);

            // after creating the order, we also need to decrease the stock of the ordered products
            foreach (var item in normalizedItems)
            {
                var objectId = ObjectId.Parse(item.ProductId);

                // we create a filter to find the specified product and also to check the available stock.
                // With this method, if the UpdateProduct modifies 0 entities, it means that stock is not available for this quantity
                // this also prevents race conditions
                var filter = Builders<Product>.Filter.And(
                    Builders<Product>.Filter.Eq(x => x.Id, objectId),
                    Builders<Product>.Filter.Gte(x => x.Stock, item.Quantity)
                );

                var updateDef = Builders<Product>.Update.Inc(x => x.Stock, -item.Quantity);
                var result = await productRepository.UpdateProduct(filter, updateDef, session);
                if (result.ModifiedCount == 0)
                {
                    await session.AbortTransactionAsync();

                    var product = productsDict[objectId];
                    return Result<OrderResponse>.Failure(
                        OrderErrors.QuantityNotAvailable(item.ProductId, product.Stock, item.Quantity));
                }
            }

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

    // Utils

    // We sum up the quantities of items that have the same id
    // if some bug on frontend happens and duplicate items get sent
    private static List<OrderItemRequest> NormalizeOrderItems(List<OrderItemRequest> orderItems)
    {
        return orderItems
            .GroupBy(x => x.ProductId)
            .Select(g => new OrderItemRequest
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                }
            ).ToList();
    }

    private static IEnumerable<ObjectId> GetMissingProducts(IEnumerable<ObjectId> requestIds,
        IEnumerable<Product> products)
    {
        return requestIds.Except(products.Select(x => x.Id));
    }

    private static (string, int, int)? CheckProductsAvailability(IEnumerable<OrderItemRequest> requestItems,
        IDictionary<ObjectId, Product> productsDict)
    {
        foreach (var item in requestItems)
        {
            var stringId = item.ProductId;
            var objectId = ObjectId.Parse(stringId);
            var product = productsDict[objectId];
            if (item.Quantity > product.Stock)
                return (stringId, product.Stock, item.Quantity);
        }

        return null;
    }
}