using MongoDB.Bson;

namespace EMerx.ResultPattern.Errors;

public static class OrderErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"Order with id {id} not found.");

    public static Error NotFound(IEnumerable<ObjectId> missingIds) =>
        new(StatusCodes.NotFound, $"These product ids do not exist: {string.Join(", ", missingIds)}");
}