using System.Text.Json;
using EMerx.DTOs.Address;
using EMerx.DTOs.OrderItems.Request;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.ApiTests.Helpers;

public static class OrderApiHelpers
{
    public static string TestProductId = "6a1d7853560f5e66666adb13";

    public static OrderRequest CreateOrderRequest(IAPIRequestContext request)
    {
        return new OrderRequest
        {
            Address = new AddressRequiredDto
            {
                City = "City",
                HouseNumber = "1234",
                Street = "Street",
            },
            Items =
            [
                new()
                {
                    Quantity = 1,
                    ProductId = TestProductId,
                }
            ],
        };
    }

    public static async Task<OrderResponse> PostOrder(IAPIRequestContext request)
    {
        var data = CreateOrderRequest(request);

        await using var response = await request.PostAsync(OrderUrls.Base, new APIRequestContextOptions
        {
            DataObject = data
        });

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return await response.JsonAsync<OrderResponse>(options);
    }

    public static async Task DeleteOrderAndProducts(IAPIRequestContext request, string orderId)
    {
        await request.DeleteAsync(orderId);
    }
}