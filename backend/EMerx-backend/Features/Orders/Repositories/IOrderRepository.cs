using MongoDB.Bson;

namespace EMerx_backend.Features.Orders.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();
    Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId);
    Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId);
    Task<Order?> GetOrderById(ObjectId id);
    Task CreateOrder(Order order);
    Task UpdateOrder(Order order);
    Task DeleteOrder(Order order);
}