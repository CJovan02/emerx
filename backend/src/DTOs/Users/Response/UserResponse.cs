using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Users.Response;

public sealed record UserResponse(
    ObjectId Id,
    string Name,
    string Surname,
    string Email,
    string Password,
    Address Address);