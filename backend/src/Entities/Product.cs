using EMerx.Domain.Models;

namespace EMerx.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }

    //should be rediscussed about categories enum
    public required string Category { get; set; }

    public ProductImage? Image { get; set; }
    public required decimal Price { get; set; }

    /// <summary>
    /// Used for calculating AverageRating
    /// </summary>
    public double SumRatings { get; set; } = 0;

    public int ReviewsCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
}