using MongoDB.Bson;

namespace EMerx.Result;

public static class ProductErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"Product with id {id} not found.");
}