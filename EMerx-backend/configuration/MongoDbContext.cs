using EMerx_backend.Entities;
using MongoDB.Driver;

namespace EMerx_backend.configuration;

public class MongoDbContext
{
    private IMongoDatabase Database { get; set; }

    public MongoDbContext(IMongoClient mongoClient, IConfiguration config)
    {
        var dbName = config["MongoDB:DatabaseName"];
        Database = mongoClient.GetDatabase(dbName);
    }

    public IMongoCollection<Product> Products => Database.GetCollection<Product>(nameof(Product) + 's');
    public IMongoCollection<User> Users => Database.GetCollection<User>(nameof(User) + 's');
    public IMongoCollection<Review> Reviews => Database.GetCollection<Review>(nameof(Review) + 's');
    public IMongoCollection<Order> Orders => Database.GetCollection<Order>(nameof(Order) + 's');
}