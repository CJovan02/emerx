using EMerx_backend.Entities;

namespace EMerx_backend.Dtos;

public record RegisterUserDto(string Name, string Surname, string Email, string Password, Address Address);