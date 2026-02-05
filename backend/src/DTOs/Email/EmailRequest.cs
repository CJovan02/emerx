using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.DTOs.Email;

public record EmailRequest
{
    [Required] [FromRoute(Name = "email")] public required string Email { get; init; }
}