using MongoDB.Bson;

namespace EMerx_backend.Entities;

public class Order : BaseEntity
{
    public ObjectId UserId { get; set; }
    public ObjectId ProductId { get; set; }
    public required Address Address { get; set; }
    public int Quantity  { get; set; }
}