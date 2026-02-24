namespace EMerx.DTOs.Products.Request;

public sealed record PatchProductRequest
{
    public string? Name { get; init; }
    public string? Category { get; init; }
    public string? Image { get; init; }
    public decimal? Price { get; init; }
}
