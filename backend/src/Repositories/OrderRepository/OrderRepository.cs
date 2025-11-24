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
        // Alternative syntax
        //var filter = Builders<Order>.Filter.Eq("Items.ProductId", productId);
        //var filter = Builders<Order>.Filter.AnyEq(x => x.Items.Select(i => i.ProductId), productId);

        var filter = Builders<Order>.Filter.ElemMatch(
            x => x.Items,
            item => item.ProductId == productId
        );

        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersForUser(ObjectId userId)
    {
        return await _orders.Find(order => order.UserId == userId).ToListAsync();
    }

    public async Task<bool> HasUserOrderedProduct(ObjectId userId, ObjectId productId,
        IClientSessionHandle? session = null)
    {
        var builder = Builders<Order>.Filter;
        // var filter = Builders<Order>.Filter.Where(o => o.ProductId == productId && o.UserId == userId);
        // var filter = builder.And(
        //     builder.Eq(o => o.UserId, userId),
        //     builder.Eq(o => o.ProductId, productId)
        // );

        // Longer syntax
        // var filter = builder.And(
        //     builder.ElemMatch(
        //         x => x.Items,
        //         item => item.ProductId == productId
        //     ),
        //     builder.Eq(x => x.UserId, userId)
        // );

        // Shorter syntax for combining filters
        var filter = builder.ElemMatch(
            x => x.Items,
            item => item.ProductId == productId
        )
        & builder.Eq(x => x.UserId, userId);

        if (session is not null)
        {
            return await _orders
                .Find(session, filter)
                .AnyAsync();
        }

        return await _orders
            .Find(filter)
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