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
}
