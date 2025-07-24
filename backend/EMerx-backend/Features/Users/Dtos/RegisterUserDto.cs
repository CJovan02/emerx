namespace EMerx_backend.Features.Users.Dtos;

public sealed record RegisterUserDto(
    string Name, 
    string Surname, 
    string Email, 
    string Password);