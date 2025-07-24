using EMerx_backend.Entities;
using MongoDB.Bson;

namespace EMerx_backend.Features.Users.Dtos;

//orderdto
//reviewdto
public sealed record UserDto(
    ObjectId Id,
    string Name,
    string Surname,
    string Email,
    string Password,
    Address Address);