using System.Text.Json;
using EMerx.Common.Filters;
using EMerx.DTOs.Orders.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.ApiTests;

[TestFixture]
public class OrderApiTest : PlaywrightTest
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
            BaseURL = ServerUrls.BackendTest,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true,
        });
    }

    [Test]
    public async Task GetAll_ValidRequest_ReturnsPagedOrders()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 3;

        // Act
        await using var response =
            await _request.GetAsync($"{OrderUrls.Base}?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var pageResponse =
            await response.JsonAsync<PageOfResponse<OrderResponse>>(_jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(pageResponse, Is.Not.Null);
            Assert.That(pageResponse.Page, Is.EqualTo(page));
            Assert.That(pageResponse.PageSize, Is.EqualTo(pageSize));
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
            await _request.GetAsync($"{OrderUrls.Base}?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task GetById_ExistingOrder_ReturnsOrder()
    {
        // Arrange
        var createdOrder =
            await OrderApiHelpers.PostOrder(_request);

        try
        {
            // Act
            await using var response =
                await _request.GetAsync($"{OrderUrls.Base}/{createdOrder.Id}");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            var order =
                await response.JsonAsync<OrderResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order.Id, Is.EqualTo(createdOrder.Id));
            });
        }
        finally
        {
            await OrderApiHelpers.DeleteOrderAndProducts(_request, createdOrder.Id);
        }
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await _request.DisposeAsync();
    }
}