using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EMerx.Entities;

public abstract class BaseEntity
{
    [BsonId]
    public ObjectId Id { get; } = ObjectId.Empty;
}