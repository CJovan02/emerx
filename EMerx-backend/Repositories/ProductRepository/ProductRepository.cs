using EMerx_backend.Dto.Product;
using EMerx_backend.Entities;
using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Repositories.ProductRepository;

public class ProductRepository(MongoDbContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _products = context.Products;

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _products.Find(product => true).ToListAsync();
    }

    public async Task<Product> GetProductById(ObjectId id)
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

    public async Task PatchProduct(PatchProductDto patchProductDto)
    {
        var updates = new List<UpdateDefinition<Product>>();
        if (patchProductDto.Name is not null)
            updates.Add(Builders<Product>.Update.Set(p => p.Name, patchProductDto.Name));
        if (patchProductDto.Category is not null)
            updates.Add(Builders<Product>.Update.Set(p => p.Category, patchProductDto.Category));
        if (patchProductDto.Image is not null)
            updates.Add(Builders<Product>.Update.Set(p => p.Image, patchProductDto.Image));
        if (patchProductDto.Price is not null)
            updates.Add(Builders<Product>.Update.Set(p => p.Price, patchProductDto.Price));

        if (updates.Count == 0) return;

        var updateDef = Builders<Product>.Update.Combine(updates);

        await _products.UpdateOneAsync(p => p.Id == patchProductDto.Id, updateDef);
    }

    public async Task DeleteProduct(ObjectId id)
    {
        await _products.DeleteOneAsync(product => product.Id == id);
    }
}