using EMerx_backend.Shared;
using FluentResults;
using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews.Errors;

public class ReviewNotFoundError : Error
{
    public ReviewNotFoundError(ObjectId id) : base($"Review with id {id} not found.")
    {
        Metadata.Add(Constants.StatusCode, 404);
    }
}