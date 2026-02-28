using System.ComponentModel.DataAnnotations;

namespace EMerx.DTOs.Address;

public sealed record AddressRequiredDto
{
    [Required] public required string City { get; init; }
    [Required] public required string Street { get; init; }
    [Required] public required string HouseNumber { get; init; }
}