namespace EMerx_backend.Infrastructure.MongoDb;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "EMerx";
}