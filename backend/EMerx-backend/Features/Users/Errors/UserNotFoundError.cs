using EMerx_backend.Shared;
using FluentResults;
using MongoDB.Bson;

namespace EMerx_backend.Features.Users.Errors;

public class UserNotFoundError : Error
{
    public UserNotFoundError(ObjectId id) : base($"User with id {id} not found.")
    {
        Metadata.Add(Constants.StatusCode, 404);
    }
}