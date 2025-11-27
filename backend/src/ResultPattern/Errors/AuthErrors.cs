using EMerx.DTOs.Users.Request;
using EMerx.Entities;

namespace EMerx.ResultPattern.Errors;

public static class AuthErrors
{
    public static Error NotFoundByUid(string uid) =>
        new(StatusCodes.BadRequest, $"Auth user {uid} doesn't exist.");
    
    public static Error NotFoundByEmail(string email) =>
        new(StatusCodes.BadRequest, $"Auth user {email} doesn't exist.");
}