namespace EMerx.Common.Filters;

public record ProductFilterParams
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public double? MinRating { get; set; }
    public bool InStockOnly { get; set; } = false;
}
