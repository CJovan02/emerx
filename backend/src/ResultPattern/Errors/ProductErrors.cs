using MongoDB.Bson;

namespace EMerx.ResultPattern.Errors;

public static class ProductErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"Product with id {id} not found.");

    public static Error NoUpdates(ObjectId id) =>
        new(StatusCodes.BadRequest, $" You provided no updates for product with id {id}.");
}