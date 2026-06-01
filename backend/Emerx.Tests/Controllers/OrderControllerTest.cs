using System.Security.Claims;
using EMerx.Common.Filters;
using EMerx.Controllers;
using EMerx.DTOs.Address;
using EMerx.DTOs.Id;
using EMerx.DTOs.OrderItems.Request;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests.Controllers;

public class OrderControllerTest
{
    private OrderController _orderController;
    private Mock<IOrderService> _orderService;

    private const string _authorizedUserEmail = "authorized@email.com";
    private const string _firebaseUid = "firebase-uid";
    private DefaultHttpContext _emptyHttpContext;
    private DefaultHttpContext _httpContextWithUser;

    [SetUp]
    public void SetUp()
    {
        _orderService = new Mock<IOrderService>();
        _orderController = new OrderController(_orderService.Object);

        var claims = new List<Claim>
        {
            new("user_id", _firebaseUid),
            new(ClaimTypes.Email, _authorizedUserEmail),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");

        _httpContextWithUser = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        _emptyHttpContext = new DefaultHttpContext();
    }

    [Test]
    public async Task GetAll_ValidPageParams_ReturnsOk()
    {
        // Arrange

        var pageParams = new PageParams
        {
            Page = 1,
            PageSize = 10
        };

        var response = new PageOfResponse<OrderResponse>(
            [],
            1,
            10,
            0);

        _orderService
            .Setup(s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize))
            .ReturnsAsync(Result<PageOfResponse<OrderResponse>>.Success(response));

        // Act

        var result = await _orderController.GetAll(pageParams);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _orderService.Verify(
            s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize),
            Times.Once);
    }

    [Test]
    public async Task GetById_ExistingOrder_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var response = CreateOrderResponse();

        _orderService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<OrderResponse>.Success(response));

        // Act

        var result = await _orderController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _orderService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GetById_OrderNotFound_ReturnsNotFound()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _orderService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<OrderResponse>.Failure(
                OrderErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _orderController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _orderService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Review_ValidRequest_ReturnsOk()
    {
        // Arrange

        var request = new OrderReviewRequest
        {
            Items =
            [
                new OrderItemRequest
                {
                    ProductId = ObjectId.GenerateNewId().ToString(),
                    Quantity = 2
                }
            ]
        };

        var response = CreateOrderReviewResponse();

        _orderService
            .Setup(s => s.GetOrderReview(request))
            .ReturnsAsync(Result<OrderReviewResponse>.Success(response));

        // Act

        var result = await _orderController.Review(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _orderService.Verify(
            s => s.GetOrderReview(request),
            Times.Once);
    }

    [Test]
    public async Task Review_ProductNotFound_ReturnsNotFound()
    {
        // Arrange

        var request = new OrderReviewRequest
        {
            Items = []
        };

        _orderService
            .Setup(s => s.GetOrderReview(request))
            .ReturnsAsync(Result<OrderReviewResponse>.Failure(
                OrderErrors.NotFound([])));

        // Act

        var result = await _orderController.Review(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _orderService.Verify(
            s => s.GetOrderReview(request),
            Times.Once);
    }

    [Test]
    public async Task Create_UserAuthenticated_ReturnsCreated()
    {
        // Arrange

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextWithUser
        };

        var request = CreateOrderRequest();

        var response = CreateOrderResponse();

        _orderService
            .Setup(s => s.CreateAsync(_firebaseUid, request))
            .ReturnsAsync(Result<OrderResponse>.Success(response));

        // Act

        var result = await _orderController.Create(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _orderService.Verify(
            s => s.CreateAsync(_firebaseUid, request),
            Times.Once);
    }

    [Test]
    public async Task Create_UserUnauthorized_ReturnsUnauthorized()
    {
        // Arrange

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = _emptyHttpContext
        };

        var request = CreateOrderRequest();

        // Act

        var result = await _orderController.Create(request);

        // Assert

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        _orderService.Verify(
            s => s.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<OrderRequest>()),
            Times.Never);
    }

    [Test]
    public async Task Delete_ExistingOrder_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _orderService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Success());

        // Act

        var result = await _orderController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _orderService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Delete_OrderNotFound_ReturnsNotFound()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _orderService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Failure(
                OrderErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _orderController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _orderService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }



    private OrderRequest CreateOrderRequest()
    {
        return new OrderRequest
        {
            Address = new AddressRequiredDto
            {
                City = "city",
                HouseNumber = "houseNumber",
                Street = "street",
            },
            Items =
            [
                new OrderItemRequest
                {
                    ProductId = ObjectId.GenerateNewId().ToString(),
                    Quantity = 2
                }
            ]
        };
    }
    private OrderResponse CreateOrderResponse()
    {
        return new OrderResponse
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = ObjectId.GenerateNewId().ToString(),
            Address = new AddressDto
            {
                City = "city",
                HouseNumber = "HouseNumber",
                Street = "Street",
            },
            PlacedAt = DateTime.Now,
            Price = 0,
            Items = []
        };
    }

    private OrderReviewResponse CreateOrderReviewResponse()
    {
        return new OrderReviewResponse
        {
            Items = [],
            Total = 0,
        };
    }
}