using MongoDB.Bson;

namespace EMerx_backend.Features.Products.Dtos;

public sealed record ProductDto(
    ObjectId Id,
    string Name,
    string Category,
    string Image,
    double Price);