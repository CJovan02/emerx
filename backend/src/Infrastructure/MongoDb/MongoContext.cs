using EMerx.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Infrastructure.MongoDb;

public class MongoContext(IOptions<MongoDbSettings> options, ILogger<MongoContext> logger)
{
    private IMongoDatabase _database;
    private MongoClient _client;
    private readonly ILogger<MongoContext> _logger = logger;

    public void Connect()
    {
        _logger.LogInformation("Connecting to MongoDB...");

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
            _logger.LogInformation("✅ Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ MongoDb connection could not be established.");
            throw ex;
        }
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");

    public Task<IClientSessionHandle> StartSessionAsync()
    {
        return _client.StartSessionAsync();
    }
}