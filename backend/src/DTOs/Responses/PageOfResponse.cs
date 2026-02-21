using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EMerx.Common.Filters;

public record PageOfResponse<T>
{
    [Required] 
    public required IEnumerable<T> Items { get; init; }

    [Required] 
    public required int Page { get; init; }

    [Required] 
    public required int PageSize { get; init; }

    [Required] 
    public required int TotalItems { get; init; }

    [Required] 
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 1;

    [Required] 
    public bool HasNextPage => Page < TotalPages;

    [Required] 
    public bool HasPreviousPage => Page > 1 && Page <= TotalPages;

    [SetsRequiredMembers]
    public PageOfResponse(IEnumerable<T> items, int page, int pageSize, int totalItems)
    {
        Items = items;
        Page = page <= 0 ? 1 : page;
        PageSize = pageSize <= 0 ? 10 : pageSize;
        TotalItems = totalItems;
    }
}