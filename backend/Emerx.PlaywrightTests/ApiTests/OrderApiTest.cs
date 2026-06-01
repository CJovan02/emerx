using System.Text.Json;
using EMerx.Common.Filters;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MongoDB.Bson;

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
                Assert.That(order.Price, Is.GreaterThan(0));
            });
        }
        finally
        {
            await OrderApiHelpers.DeleteOrder(_request, createdOrder.Id);
        }
    }

    [Test]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response =
            await _request.GetAsync($"{OrderUrls.Base}/{nonExistingId}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task Overview_ValidRequest_ReturnsOrderReview()
    {
        // Arrange
        var request =
            OrderApiHelpers.CreateReviewRequest();

        // Act
        await using var response =
            await _request.PostAsync(
                OrderUrls.Overview,
                new()
                {
                    DataObject = request
                });

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var review =
            await response.JsonAsync<OrderReviewResponse>(_jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(review, Is.Not.Null);
            Assert.That(review.Total, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task Overview_EmptyCart_ReturnsBadRequest()
    {
        // Arrange
        var request = new OrderReviewRequest
        {
            Items = []
        };

        // Act
        await using var response =
            await _request.PostAsync(
                OrderUrls.Overview,
                new()
                {
                    DataObject = request
                });

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreatedOrder()
    {
        // Arrange
        var request =
            OrderApiHelpers.CreateOrderRequest();

        OrderResponse? createdOrder = null;

        try
        {
            // Act
            await using var response =
                await _request.PostAsync(
                    OrderUrls.Base,
                    new()
                    {
                        DataObject = request
                    });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            createdOrder =
                await response.JsonAsync<OrderResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(createdOrder, Is.Not.Null);
                Assert.That(createdOrder.Items.Count, Is.EqualTo(1));
            });
        }
        finally
        {
            if (createdOrder is not null)
                await OrderApiHelpers.DeleteOrder(
                    _request,
                    createdOrder.Id);
        }
    }

    [Test]
    public async Task Delete_ExistingOrder_ReturnsSuccess()
    {
        // Arrange
        var createdOrder =
            await OrderApiHelpers.PostOrder(_request);

        // Act
        await using var deleteResponse =
            await _request.DeleteAsync(
                $"{OrderUrls.Base}/{createdOrder.Id}");

        // Assert
        Assert.That(deleteResponse.Status, Is.EqualTo(200));

        // Verify deletion
        await using var getResponse =
            await _request.GetAsync(
                $"{OrderUrls.Base}/{createdOrder.Id}");

        Assert.That(getResponse.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task Delete_NonExistingOrder_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var deleteResponse =
            await _request.DeleteAsync(
                $"{OrderUrls.Base}/{nonExistingId}");

        // Assert
        Assert.That(deleteResponse.Status, Is.EqualTo(404));
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await _request.DisposeAsync();
    }
}