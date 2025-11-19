using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Users;

public sealed record UserResponseDto(
    ObjectId Id,
    string Name,
    string Surname,
    string Email,
    string Password,
    Address Address);