using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.Repositories.OrderRepository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();

    Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId);

    Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId);

    Task<Order?> GetOrderById(ObjectId id);

    Task CreateOrder(Order order);

    Task UpdateOrder(Order order);

    Task DeleteOrder(ObjectId id);
}