using System.ComponentModel.DataAnnotations;
using EMerx.DTOs.Address;
using EMerx.Entities;

namespace EMerx.DTOs.Users.Request;

public record UpdateUserRequest
{
    [Required] public required string Name { get; init; }
 
    [Required] public required string Surname { get; init; }
    
    public AddressDto Address { get; init; }
}