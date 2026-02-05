using System.ComponentModel.DataAnnotations;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders.Response;

public sealed record OrderResponse
{
    [Required] public required ObjectId Id { get; init; }
    [Required] public required ObjectId UserId { get; init; }
    [Required] public required List<OrderItem> Items { get; init; }
    [Required] public required Address Address { get; init; }
    [Required] public required decimal Price { get; init; }
    [Required] public required DateTime PlacedAt { get; init; }
}