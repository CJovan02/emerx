using FluentResults;
using MongoDB.Bson;

namespace EMerx_backend.Features.Users.Errors;

public class UserNotFoundError : Error
{
    public UserNotFoundError(ObjectId id) : base($"User with id {id} not found.")
    {
        Metadata.Add("StatusCode", 404);
    }
}