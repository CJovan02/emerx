using EMerx.Entities;
using MongoDB.Driver;

namespace EMerx.Infrastructure.MongoDb;

public interface IMongoContext
{
    IMongoCollection<Product> Products { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<Review> Reviews { get; }
    IMongoCollection<Order> Orders { get; }

    Task<IClientSessionHandle> StartSessionAsync();
}