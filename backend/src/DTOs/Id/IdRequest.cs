using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EMerx.DTOs.Id;

public record IdRequest
{
    [Required]
    [FromRoute(Name = "id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; init; }
}