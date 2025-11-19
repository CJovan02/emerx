using MongoDB.Bson;

namespace EMerx.Result;

public static class UserErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"User with id {id} not found.");
    public static Error EmailOccupied(string email) => new(StatusCodes.Conflict, $"Email {email} is already taken.");
}