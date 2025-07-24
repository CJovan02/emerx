using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Features.Products.Repositories;

public class ProductRepository(MongoDbContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _products = context.Products;

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _products.Find(product => true).ToListAsync();
    }

    public async Task<Product?> GetProductById(ObjectId id)
    {
        return await _products.Find(product => product.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateProduct(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateProduct(Product product)
    {
        await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
    }

    public async Task DeleteProduct(ObjectId id)
    {
        await _products.DeleteOneAsync(product => product.Id == id);
    }
}