using System.Text.Json;
using EMerx.DTOs.Products.Response;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.ApiTests.Helpers;

public static class ProductApiHelpers
{
    public static IFormData CreateProductFormData
        (IAPIRequestContext request, string productName = "Test Name", int stock = 1)
    {
        var form = request.CreateFormData();
        form.Append("Name", productName);
        form.Append("Description", "Test Description");
        form.Append("Category", "Test Category");
        form.Append("Price", 10);
        form.Append("Stock", stock);

        return form;
    }

    public static IFormData CreatePatchProductFormData
        (IAPIRequestContext request, string productName, int price)
    {
        var form = request.CreateFormData();
        form.Append("Name", productName);
        form.Append("Price", price);
        form.Append("Image.HasValue", false);

        return form;
    }


    public static async Task<ProductResponse> PostProduct(IAPIRequestContext request, string productName = "Test Name")
    {
        var form = CreateProductFormData(request, productName);

        await using var response = await request.PostAsync(ProductUrls.Base, new APIRequestContextOptions
        {
            Multipart = form
        });

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return await response.JsonAsync<ProductResponse>(options);
    }

    public static async Task DeleteProduct(IAPIRequestContext request, string productId)
    {
        await request.DeleteAsync($"{ProductUrls.Base}/{productId}");
    }
}