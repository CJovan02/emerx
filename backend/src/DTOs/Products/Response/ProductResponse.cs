using MongoDB.Bson;

namespace EMerx.DTOs.Products.Response;

public sealed record ProductResponse(
    ObjectId Id,
    string Name,
    string Category,
    string? Image,
    decimal Price);