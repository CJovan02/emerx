namespace EMerx_backend.Features.Products.Dtos;

public sealed record CreateProductDto(
    string Name,
    string Category,
    string Image,
    double Price);