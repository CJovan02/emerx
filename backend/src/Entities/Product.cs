namespace EMerx.Entities;

public class Product : BaseEntity
{
    public required string Name { get; init; }

    //should be rediscussed about categories enum
    public required string Category { get; init; }

    //will be changed
    public string? Image { get; init; }
    public double Price { get; init; }

    public int ReviewsCount { get; init; } = 0;

    public double AverageRating { get; init; } = 0;
}