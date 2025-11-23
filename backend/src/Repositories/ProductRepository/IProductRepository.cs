using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.Repositories.ProductRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProductById(ObjectId id);
    Task CreateProduct(Product product);
    Task UpdateProduct(Product product);
    Task UpdateProductReviewAsync(ObjectId productId, double averageRating, double sumRatings, int reviewsCount);
    Task DeleteProduct(ObjectId id);
}