using EMerx.Domain.Models;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Products;

public static class ProductsExtensions
{
    public static Product ToDomain(this CreateProductRequest request, ObjectId? objectId = null,
        string? imageVersion = null)
    {
        return new Product
        {
            Id = objectId ?? ObjectId.GenerateNewId(),
            Name = request.Name,
            Category = request.Category,
            Price = request.Price,
            Description = request.Description,
            Stock =  request.Stock,
            ImageVersion = imageVersion,
        };
    }

    public static ProductResponse ToResponse(this Product domain, string? imageUrl = null)
    {
        return new ProductResponse
        {
            Id = domain.Id.ToString(),
            Name = domain.Name,
            Category = domain.Category,
            Price = domain.Price,
            AverageRating = domain.AverageRating,
            Description = domain.Description,
            ReviewCount =  domain.ReviewCount,
            Stock = domain.Stock,
            ThumbnailUrl = imageUrl,
        };
    }

    public static ProductImageResponse ToResponse(this ProductImage domain, string url)
    {
        return new ProductImageResponse
        {
            Order = domain.Order,
            PublicId = domain.PublicId,
            IsThumbnail = domain.isThumbnail,
            Url = url
        };
    }
}