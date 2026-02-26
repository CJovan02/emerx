namespace EMerx.Domain.Models;

public class ProductImage
{
    public required string PublicId { get; set; }
    public required bool isThumbnail { get; set; } = false;
    public required int Order { get; set; } = 0;
}