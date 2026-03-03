using EMerx.DTOs.Address;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.DTOs.Orders;

public static class OrdersExtensions
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Items = order.Items,
            Address = order.Address.ToDto()!,
            Price = order.Price,
            PlacedAt = order.PlacedAt
        };
    }

    public static OrderReviewResponse ToResponse(this OrderReviewRequest request,
        IDictionary<ObjectId, Product> productsDict, Func<string, string, string> buildImageUrl)
    {
        var domainItems = request.Items.Select(item =>
        {
            var id = item.ProductId;
            var objectId = ObjectId.Parse(id);
            var product = productsDict[objectId];

            var imageUrl = product.ImageVersion is not null
                ? buildImageUrl(id, product.ImageVersion)
                : null;
            var lineTotal = item.Quantity * product.Price;

            return new OrderReviewItem
            {
                ProductId = id,
                ProductName = product.Name,
                ImageUrl = imageUrl,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                Stock = product.Stock,
                LineTotal = lineTotal,
            };
        }).ToList();
        var totalPrice = domainItems.Sum(item => item.LineTotal);

        return new OrderReviewResponse
        {
            Items = domainItems,
            Total = totalPrice
        };
    }

    public static Order ToDomain(this OrderRequest order, IDictionary<ObjectId, Product> productsDict, ObjectId userId)
    {
        var domainItems = order.Items.Select(item =>
        {
            var productId = ObjectId.Parse(item.ProductId);
            var product = productsDict[productId];

            return new OrderItem
            {
                ProductId = ObjectId.Parse(item.ProductId),
                Name = product.Name,
                PriceAtOrder = product.Price,
                Quantity = item.Quantity,
            };
        }).ToList();

        return new Order
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            Items = domainItems,
            Address = order.Address.ToEntity(),
            Price = domainItems.Sum(x => x.PriceAtOrder * x.Quantity)
        };
    }
}