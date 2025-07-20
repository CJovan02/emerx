using EMerx_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace EMerx_backend.Repositories.ProductRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product> GetProductById(ObjectId id);
    Task CreateProduct(Product product);
    Task UpdateProduct(Product product);
    Task DeleteProduct(ObjectId id);
}