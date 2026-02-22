namespace EMerx.Common.Filters;

public record PageOf<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 1;
    
    public bool HasNextPage => Page < TotalPages;
    
    public bool HasPreviousPage => Page > 1 && Page <= TotalPages;
}