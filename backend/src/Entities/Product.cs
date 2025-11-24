namespace EMerx.Entities;

public class Product : BaseEntity
{
    public required string Name { get; init; }

    //should be rediscussed about categories enum
    public required string Category { get; init; }

    //will be changed
    public string? Image { get; init; }
    public decimal Price { get; init; }

    /// <summary>
    /// Used for calculating AverageRating
    /// </summary>
    public double SumRatings { get; init; } = 0;
    public int ReviewsCount { get; init; } = 0;

    public double AverageRating { get; init; } = 0;
}