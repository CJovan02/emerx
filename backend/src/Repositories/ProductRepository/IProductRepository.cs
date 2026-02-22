using EMerx.Common.Filters;
using EMerx.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ProductRepository;

public interface IProductRepository
{
    Task<PageOf<Product>> GetProducts(int page, int pageSize);
    Task<Product?> GetProductById(ObjectId id, IClientSessionHandle? session = null);
    Task<IEnumerable<Product>> GetProductsByIds (IEnumerable<ObjectId> ids, IClientSessionHandle? session = null);
    Task CreateProduct(Product product);
    Task UpdateProduct(Product product);

    Task UpdateProductReviewAsync(ObjectId productId, double averageRating, double sumRatings, int reviewsCount,
        IClientSessionHandle? session = null);

    Task DeleteProduct(ObjectId id);
}