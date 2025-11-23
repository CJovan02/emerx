using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.Repositories.OrderRepository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();

    Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId);

    Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId);

    /// <summary>
    /// Checks if user has at least 1 order on product
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<bool> HasUserOrderedProduct(ObjectId userId, ObjectId productId);

    Task<Order?> GetOrderById(ObjectId id);

    Task CreateOrder(Order order);

    Task UpdateOrder(Order order);

    Task DeleteOrder(ObjectId id);
}