using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Users.Request;

public sealed record RegisterUser
{
    [Required] public required string Name { get; init; }
    [Required] public required string Surname { get; init; }
    [Required] public required string Email { get; init; }
    [Required] public required string Password { get; init; }
}