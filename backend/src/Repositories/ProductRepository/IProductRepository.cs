using EMerx.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Repositories.ProductRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProductById(ObjectId id, IClientSessionHandle? session = null);
    Task CreateProduct(Product product);
    Task UpdateProduct(Product product);

    Task UpdateProductReviewAsync(ObjectId productId, double averageRating, double sumRatings, int reviewsCount,
        IClientSessionHandle? session = null);

    Task DeleteProduct(ObjectId id);
}