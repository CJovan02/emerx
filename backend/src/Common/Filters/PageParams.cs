namespace EMerx.Common.Filters;

public record PageParams
{
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 0;
}