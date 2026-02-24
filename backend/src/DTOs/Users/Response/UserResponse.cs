using System.ComponentModel.DataAnnotations;
using EMerx.DTOs.Address;

namespace EMerx.DTOs.Users.Response;

public sealed record UserResponse
{
    [Required] public string Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required string Surname { get; init; }
    [Required] public required string Email { get; init; }
    [Required] public required AddressDto Address { get; init; }
}