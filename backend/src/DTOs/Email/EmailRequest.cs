using Microsoft.AspNetCore.Mvc;

namespace EMerx.DTOs.Email;

public record EmailRequest
{
    [FromRoute(Name = "email")] public required string Email { get; init; }
}