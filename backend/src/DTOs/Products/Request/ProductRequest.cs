namespace EMerx.DTOs.Products.Request;

public sealed record ProductRequest(
    string Name,
    string Category,
    string Image,
    double Price);