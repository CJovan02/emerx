using System.Text.Json;
using EMerx.Common.Filters;
using EMerx.DTOs.Products.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MongoDB.Bson;

namespace Emerx.PlaywrightTests.ApiTests;

[TestFixture]
public class ProductsApiTest : PlaywrightTest
{
    private IAPIRequestContext _request;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    [SetUp]
    public async Task SetUp()
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };

        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ApiTestUrls.Products,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true,
        });
    }

    [Test]
    public async Task GetPaged_ValidPage_ReturnsPageOfProducts()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 3;

        // Act
        await using var response =
            await _request.GetAsync($"?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var pageResponse = await response.JsonAsync<PageOfResponse<ProductResponse>>(_jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(pageResponse, Is.Not.Null);
            Assert.That(pageResponse.Items, Is.Not.Null);
            Assert.That(pageResponse.Page, Is.EqualTo(page));
            Assert.That(pageResponse.PageSize, Is.EqualTo(pageSize));
            Assert.That(pageResponse.Items.Count, Is.LessThanOrEqualTo(pageSize));
        });
    }

    [Test]
    public async Task GetPaged_InvalidPage_ReturnsBadRequest()
    {
        // Arrange
        const int page = 0;
        const int pageSize = -5;

        // Act
        await using var response =
            await _request.GetAsync($"?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task GetCategories()
    {
        // Arrange, Act
        await using var response = await _request.GetAsync(ApiTestUrls.ProductCategoriesRaw);

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));
        var categories = await response.JsonAsync<List<string>>();

        Assert.That(categories, Is.Not.Empty);
    }

    [Test]
    public async Task GetById_ValidId_ReturnsProduct()
    {
        // Arrange
        var productResponse = await ProductApiHelpers.PostProduct(_request);
        var productId = productResponse.Id;

        // Act
        try
        {
            await using var response =
                await _request.GetAsync(productId);

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            var product = await response.JsonAsync<ProductResponse>(_jsonOptions);
            Assert.Multiple(() =>
            {
                Assert.That(product, Is.Not.Null);
                Assert.That(product.Id, Is.EqualTo(productId));
            });
        }
        // cleanup
        finally
        {
            await ProductApiHelpers.DeleteProduct(_request, productId);
        }
    }

    [Test]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response =
            await _request.GetAsync(nonExistingId);

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task CreateProduct_ValidParameters_ReturnsNewProduct()
    {
        // Arrange
        const string productName = "Test Product";
        const int productStock = 1;

        var form = ProductApiHelpers.CreateProductFormData(_request, productName, productStock);

        ProductResponse productResponse = null;
        try
        {
            // Act
            await using var response = await _request.PostAsync("", new APIRequestContextOptions
            {
                Multipart = form
            });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            productResponse = await response.JsonAsync<ProductResponse>(_jsonOptions);
            Assert.Multiple(() =>
            {
                Assert.That(productResponse, Is.Not.Null);
                Assert.That(productResponse.Name, Is.EqualTo(productName));
                Assert.That(productResponse.Stock, Is.EqualTo(productStock));
                Assert.That(productResponse.AverageRating, Is.EqualTo(0));
            });
        }
        finally
        {
            if (productResponse is not null)
                await ProductApiHelpers.DeleteProduct(_request, productResponse.Id);
        }
    }

    [Test]
    public async Task CreateProduct_InvalidStock_ReturnsBadRequest()
    {
        // Arrange
        const string productName = "Test Product";
        const int productStock = -1;

        var form = ProductApiHelpers.CreateProductFormData(_request, productName, productStock);

        ProductResponse productResponse = null;
        // Act
        await using var response = await _request.PostAsync("", new APIRequestContextOptions
        {
            Multipart = form
        });

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task PatchProduct_ValidParameters_ReturnsProduct()
    {
        // Arrange
        var createdProduct = await ProductApiHelpers.PostProduct(_request);

        var newName = "New Name";
        int newPrice = 10;
        var form = ProductApiHelpers.CreatePatchProductFormData(_request, newName, newPrice);

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync(createdProduct.Id, new APIRequestContextOptions
                {
                    Multipart = form
                });

            // Assert
            // Api only returns OK when success without the updated object.
            // So that's the only thing we can assert
            Assert.That(response.Status, Is.EqualTo(200));
        }
        finally
        {
            await ProductApiHelpers.DeleteProduct(_request, createdProduct.Id);
        }
    }

    [Test]
    public async Task PatchProduct_InvalidPrice_ReturnsBadRequest()
    {
        // Arrange
        var createdProduct = await ProductApiHelpers.PostProduct(_request);

        var newName = "New Name";
        int newPrice = -10;
        var form = ProductApiHelpers.CreatePatchProductFormData(_request, newName, newPrice);

        // Act
        try
        {
            await using var response =
                await _request.PatchAsync(createdProduct.Id, new APIRequestContextOptions
                {
                    Multipart = form
                });

            // Assert
            Assert.That(response.Status, Is.EqualTo(400));
        }
        finally
        {
            await ProductApiHelpers.DeleteProduct(_request, createdProduct.Id);
        }
    }

    [Test]
    public async Task DeleteProduct_ValidId_ReturnsOk()
    {
        // Arrange
        var productResponse = await ProductApiHelpers.PostProduct(_request);

        // Act
        await using var response = await _request.DeleteAsync(productResponse.Id);

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        await using var getResponse =
            await _request.GetAsync(productResponse.Id);

        Assert.That(getResponse.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task DeleteProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response = await _request.DeleteAsync(nonExistingId);

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }


    [TearDown]
    public async Task TearDownAPITesting()
    {
        await _request.DisposeAsync();
    }
}