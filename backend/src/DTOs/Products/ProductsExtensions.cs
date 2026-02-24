using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Products;

public static class ProductsExtensions
{
    public static Product ToDomain(this CreateProductRequest request)
    {
        return new Product
        {
            Id = new ObjectId(),
            Name = request.Name,
            Category = request.Category,
            Price = request.Price,
            Image = request.Image,
        };
    }

    public static ProductResponse ToResponse(this Product domain)
    {
        return new ProductResponse
        {
            Id = domain.Id.ToString(),
            Name = domain.Name,
            Category = domain.Category,
            Image = domain.Image,
            Price = domain.Price,
            AverageRating = domain.AverageRating
        };
    }
}