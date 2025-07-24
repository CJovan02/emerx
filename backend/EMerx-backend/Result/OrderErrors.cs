using MongoDB.Bson;


public static class OrderErrors
{
    public static Error NotFound(ObjectId id) => new(StatusCodes.NotFound, $"Order with id {id} not found.");
}