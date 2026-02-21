using System.ComponentModel.DataAnnotations;

namespace EMerx.Common.Filters;

public record PageParams
{
    public int PageSize { get; set; } = 0;
    public int Page { get; set; } = 10;
}