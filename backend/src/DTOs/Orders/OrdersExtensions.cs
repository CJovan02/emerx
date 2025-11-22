using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders;

public static class OrdersExtensions
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.UserId,
            order.ProductId,
            order.Address,
            order.Quantity);
    }

    public static Order ToDomain(this OrderRequest order)
    {
        return new Order
        {
            Id = ObjectId.GenerateNewId(),
            Address = order.Address,
            Quantity = order.Quantity,
            PlacedAt = DateTime.UtcNow,
            ProductId = ObjectId.Parse(order.ProductId),
            UserId = ObjectId.Parse(order.UserId)
        };
    }
}
