namespace EMerx.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }

    //should be rediscussed about categories enum
    public required string Category { get; set; }

    /// <summary>
    /// Using the ProductId we can easily find the image in asset library. But we still need to store the information
    /// about the image existing or not. If the image version is null, product doesn't have an image, if it's not null
    /// product has the image. Previously I just used bool value for this purpose but in order to invalidate the cache of
    /// the old image Cloudinary requires us to store the version of the image
    /// </summary>
    public required string? ImageVersion { get; set; } = null;

    public required decimal Price { get; set; }

    /// <summary>
    /// Used for calculating AverageRating
    /// </summary>
    public double SumRatings { get; set; } = 0;

    public int ReviewsCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0;
}