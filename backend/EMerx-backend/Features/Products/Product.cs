using EMerx_backend.Shared;

namespace EMerx_backend.Features.Products;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    //should be rediscussed about categories enum
    public required string Category { get; set; }
    //will be changed
    public string? Image { get; set; }
    public double Price { get; set; }
}