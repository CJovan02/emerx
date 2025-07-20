using EMerx_backend.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Infrastructure.MongoDb;

public class MongoDbContext
{
    private IMongoDatabase Database { get; }

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(mongoSettings);

        if (settings.Value.TestConnectionWithPing)
        {
            try
            {
                client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        Database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Product> Products => Database.GetCollection<Product>(nameof(Product) + 's');
    public IMongoCollection<User> Users => Database.GetCollection<User>(nameof(User) + 's');
    public IMongoCollection<Review> Reviews => Database.GetCollection<Review>(nameof(Review) + 's');
    public IMongoCollection<Order> Orders => Database.GetCollection<Order>(nameof(Order) + 's');
}