namespace EMerx.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }

    //should be rediscussed about categories enum
    public required string Category { get; set; }

    /// <summary>
    /// Using the ProductId we can easily find the image in asset library. But we still need to store the information
    /// about the image existing or not.
    /// </summary>
    public required bool HasImage { get; set; } = false;
    public required decimal Price { get; set; }

    /// <summary>
    /// Used for calculating AverageRating
    /// </summary>
    public double SumRatings { get; set; } = 0;

    public int ReviewsCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
}