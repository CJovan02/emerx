using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ProductRepository;

public class ProductRepository(MongoDbContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _products = context.Products;

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _products.Find(product => true).ToListAsync();
    }

    public async Task<Product?> GetProductById(ObjectId id, IClientSessionHandle? session = null)
    {
        var filter = Builders<Product>.Filter.Where(product => product.Id == id);

        if (session is not null)
        {
            return await _products.Find(session, filter).FirstOrDefaultAsync();
        }

        return await _products.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<ObjectId> ids,
        IClientSessionHandle? session = null)
    {
        var filter = Builders<Product>.Filter.In(x => x.Id, ids);

        if (session is not null)
        {
            return await _products.Find(session, filter).ToListAsync();
        }

        return await _products.Find(filter).ToListAsync();
    }

    public async Task CreateProduct(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateProduct(Product product)
    {
        await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
    }

    public async Task UpdateProductReviewAsync(ObjectId productId, double averageRating, double sumRatings,
        int reviewsCount, IClientSessionHandle? session = null)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
        var update = Builders<Product>.Update
            .Set(p => p.AverageRating, averageRating)
            .Set(p => p.SumRatings, sumRatings)
            .Set(p => p.ReviewsCount, reviewsCount);

        if (session is not null)
        {
            await _products.UpdateOneAsync(session, filter, update);
            return;
        }

        await _products.UpdateOneAsync(filter, update);
    }

    public async Task DeleteProduct(ObjectId id)
    {
        await _products.DeleteOneAsync(product => product.Id == id);
    }
}