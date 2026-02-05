using System.ComponentModel.DataAnnotations;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Users.Response;

public sealed record UserResponse
{
    [Required] public required ObjectId Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required string Surname { get; init; }
    [Required] public required string Email { get; init; }
    [Required] public required string Password { get; init; }
    [Required] public required Address Address { get; init; }
}