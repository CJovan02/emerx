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
        return await _orders.Find(order => true).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersForProduct(ObjectId productId)
    {
        return await _orders.Find(order => order.ProductId == productId).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId)
    {
        return await _orders.Find(order => order.UserId == userId).ToListAsync();
    }

    public async Task<Order> GetOrderById(ObjectId id)
    {
        return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateOrder(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateOrder(Order order)
    {
        await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
    }

    public async Task DeleteOrder(Order order)
    {
        await _orders.DeleteOneAsync(o => o.Id == order.Id);
    }
}