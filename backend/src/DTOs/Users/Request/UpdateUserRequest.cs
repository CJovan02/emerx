using System.ComponentModel.DataAnnotations;
using EMerx.Entities;

namespace EMerx.DTOs.Users.Request;

public record UpdateUserRequest
{
    [Required] public required string Name { get; init; }
 
    [Required] public required string Surname { get; init; }
    
    public Address Address { get; init; }
}