using MongoDB.Bson;

namespace EMerx_backend.Dto.Product;

public class PatchProductDto
{
    public required ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Image { get; set; }
    public double? Price { get; set; }
}