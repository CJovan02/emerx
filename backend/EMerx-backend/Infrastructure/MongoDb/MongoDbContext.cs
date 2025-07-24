using EMerx_backend.Entities;
using EMerx_backend.Features.Users;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Infrastructure.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoClient _client;
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
        mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(mongoSettings);

        _database = _client.GetDatabase(settings.DatabaseName);
    }

    public async Task PingAsync()
    {
        try
        {
            await _client.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("✅ Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ MongoDb connection could not be established.");
            Console.WriteLine(ex);
        }
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
}