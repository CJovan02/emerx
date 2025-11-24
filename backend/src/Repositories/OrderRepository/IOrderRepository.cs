using EMerx.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.OrderRepository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();

    Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId);

    Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId);

    /// <summary>
    /// Checks if user has at least 1 order on product
    /// </summary>
    Task<bool> HasUserOrderedProduct(ObjectId userId, ObjectId productId, IClientSessionHandle? session = null);

    Task<Order?> GetOrderById(ObjectId id);

    Task CreateOrder(Order order, IClientSessionHandle? session = null);

    Task UpdateOrder(Order order);

    Task DeleteOrder(ObjectId id);
}