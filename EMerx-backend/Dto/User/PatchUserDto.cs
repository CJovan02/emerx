using EMerx_backend.Entities;
using MongoDB.Bson;

namespace EMerx_backend.Dto.User;

public class PatchUserDto
{
    public required ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public Address? Address { get; set; }
}