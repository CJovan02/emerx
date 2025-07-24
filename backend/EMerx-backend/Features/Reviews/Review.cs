using EMerx_backend.Shared;
using MongoDB.Bson;

namespace EMerx_backend.Features.Reviews;

public class Review : BaseEntity
{
    public ObjectId UserId { get; set; }
    public ObjectId ProductId { get; set; }
    public double Rating { get; set; }
    public string? Description { get; set; }
}