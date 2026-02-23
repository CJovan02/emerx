using System.ComponentModel.DataAnnotations;
using EMerx.Entities;

namespace EMerx.DTOs.Users.Response;

public sealed record UserResponse
{
    [Required] public required string Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required string Surname { get; init; }
    [Required] public required string Email { get; init; }
    public Address? Address { get; init; }
}