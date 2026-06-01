namespace EMerx.Auth;

public record AuthUser(
    string Uid,
    string Name,
    string Email,
    Roles Role
);