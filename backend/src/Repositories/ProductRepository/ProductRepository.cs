using EMerx.Common.Filters;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ProductRepository;

public class ProductRepository(MongoContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _products = context.Products;

    public Task<bool> ProductExists(ObjectId id)
    {
        return _products.Find(product => product.Id == id).AnyAsync();
    }

    public async Task<PageOf<Product>> GetProducts(int page, int pageSize, ProductFilterParams filters)
    {
        var filter = Builders<Product>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(filters.Search))
            filter &= Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(filters.Search, "i"));

        if (!string.IsNullOrWhiteSpace(filters.Category))
            filter &= Builders<Product>.Filter.Eq(p => p.Category, filters.Category);

        if (filters.MinPrice.HasValue)
            filter &= Builders<Product>.Filter.Gte(p => p.Price, filters.MinPrice.Value);

        if (filters.MaxPrice.HasValue)
            filter &= Builders<Product>.Filter.Lte(p => p.Price, filters.MaxPrice.Value);

        if (filters.MinRating.HasValue)
            filter &= Builders<Product>.Filter.Gte(p => p.AverageRating, filters.MinRating.Value);

        if (filters.InStockOnly)
            filter &= Builders<Product>.Filter.Gt(p => p.Stock, 0);

        var totalItems = await _products.CountDocumentsAsync(filter);

        var products = await _products
            .Find(filter)
            .SortBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PageOf<Product>(products, page, pageSize, (int)totalItems);
    }

    public async Task<IEnumerable<string>> GetDistinctCategories()
    {
        return await _products
            .Distinct<string>(nameof(Product.Category), Builders<Product>.Filter.Empty)
            .ToListAsync();
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
            .Set(p => p.ReviewCount, reviewsCount);

        if (session is not null)
        {
            await _products.UpdateOneAsync(session, filter, update);
            return;
        }

        await _products.UpdateOneAsync(filter, update);
    }

    public Task UpdateProduct(ObjectId id, UpdateDefinition<Product> updateDef)
    {
        return _products.UpdateOneAsync(item => item.Id == id, updateDef);
    }

    public async Task DeleteProduct(ObjectId id)
    {
        await _products.DeleteOneAsync(product => product.Id == id);
    }
}