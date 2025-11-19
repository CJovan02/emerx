namespace EMerx.DTOs.Users;

public sealed record RegisterUserDto(
    string Name, 
    string Surname, 
    string Email, 
    string Password);