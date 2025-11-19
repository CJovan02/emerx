namespace EMerx.DTOs.Products;

public sealed record ProductRequestDto(
    string Name,
    string Category,
    string Image,
    double Price);