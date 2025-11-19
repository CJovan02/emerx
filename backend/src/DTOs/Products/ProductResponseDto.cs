using MongoDB.Bson;

namespace EMerx.DTOs.Products;

public sealed record ProductResponseDto(
    ObjectId Id,
    string Name,
    string Category,
    string Image,
    double Price);