using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.OrderRepository;

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

    public async Task<bool> HasUserOrderedProduct(ObjectId userId, ObjectId productId,
        IClientSessionHandle? session = null)
    {
        if (session is not null)
        {
            return await _orders
                .Find(session, o => o.ProductId == productId && o.UserId == userId)
                .AnyAsync();
        }

        return await _orders
            .Find(o => o.ProductId == productId && o.UserId == userId)
            .AnyAsync();
    }

    public async Task<Order?> GetOrderById(ObjectId id)
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

    public async Task DeleteOrder(ObjectId id)
    {
        await _orders.DeleteOneAsync(o => o.Id == id);
    }
}