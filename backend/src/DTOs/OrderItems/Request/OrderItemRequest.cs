namespace EMerx.DTOs.OrderItems.Request;

public sealed record OrderItemRequest(
    string ProductId,
    int Quantity
);