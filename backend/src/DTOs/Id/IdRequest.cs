using Microsoft.AspNetCore.Mvc;

namespace EMerx.DTOs.Id;

public record IdRequest
{
    [FromRoute(Name = "id")] public required string Id { get; init; }
}