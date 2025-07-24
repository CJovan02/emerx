using EMerx_backend.Shared;
using FluentResults;
using MongoDB.Bson;

namespace EMerx_backend.Features.Orders.Errors;

public class OrderNotFoundError : Error
{
    public OrderNotFoundError(ObjectId id) : base($"Order with id {id} not found.")
    {
        Metadata.Add(Constants.StatusCode, 404);
    }
}