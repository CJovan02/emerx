using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.DTOs.Id;

public record IdRequest
{
    [Required] [FromRoute(Name = "id")] public required string Id { get; init; }
}