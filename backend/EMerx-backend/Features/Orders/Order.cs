using EMerx_backend.Shared;
using MongoDB.Bson;

namespace EMerx_backend.Features.Orders;

public class Order : BaseEntity
{
    public ObjectId UserId { get; set; }
    public ObjectId ProductId { get; set; }
    public required Address Address { get; set; }
    public int Quantity  { get; set; }
    public DateTime PlacedAt { get; init; } =  DateTime.Now;
}