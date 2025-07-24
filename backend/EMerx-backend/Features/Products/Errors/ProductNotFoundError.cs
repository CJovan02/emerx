using EMerx_backend.Shared;
using FluentResults;
using MongoDB.Bson;

namespace EMerx_backend.Features.Products.Errors;

public class ProductNotFoundError : Error
{
    public ProductNotFoundError(ObjectId id) : base($"Product with id {id} not found.")
    {
        Metadata.Add(Constants.StatusCode, 404);
    }
}