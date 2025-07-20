using EMerx_backend.Entities;
using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Repositories.OrderRepository;

public class OrderRepository(MongoDbContext context) : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders = context.Orders;

    public async Task<IEnumerable<Order>> GetOrders()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> GetOrderById(ObjectId id)
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrder(Order order)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateOrder(Order order)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteOrder(Order order)
    {
        throw new NotImplementedException();
    }
}