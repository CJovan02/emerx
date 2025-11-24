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
            Id: order.Id,
            UserId: order.UserId,
            Items: order.Items,
            Address: order.Address,
            Price: order.Price,
            PlacedAt: order.PlacedAt
        );
    }

    public static Order ToDomain(this OrderRequest order, IEnumerable<Product> products)
    {
        // We create lookup (or dictionary) for easy access by the productId
        var productsLookup = products.ToLookup(x => x.Id);

        var domainItems = order.Items.Select(x =>
        {
            var productId = ObjectId.Parse(x.ProductId);
            var product = productsLookup[productId].First();

            return x.ToDomain(product);
        }).ToList();

        return new Order
        {
            Id = ObjectId.GenerateNewId(),
            UserId = ObjectId.Parse(order.UserId),
            Items = domainItems,
            Address = order.Address,
            Price = domainItems.Sum(x => x.PriceAtOrder * x.Quantity)
        };
    }
}

public static class OrderItemExtensions
{
    public static OrderItem ToDomain(this OrderItemRequest item, Product product)
    {
        return new OrderItem
        {
            ProductId = ObjectId.Parse(item.ProductId),
            Name = product.Name,
            PriceAtOrder = product.Price,
            Quantity = item.Quantity,
        };
    }
}