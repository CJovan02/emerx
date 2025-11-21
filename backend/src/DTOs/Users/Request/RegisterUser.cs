namespace EMerx.DTOs.Users.Request;

public sealed record RegisterUser(
    string Name,
    string Surname,
    string Email,
    string Password);