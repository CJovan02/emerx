using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Orders;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests;

public class OrderServiceTest
{
    private OrderService _orderService;
    private Mock<IOrderRepository> _orderRepository;
    private Mock<IUserRepository> _userRepository;
    private Mock<IProductRepository> _productRepository;
    private Mock<ICloudinaryRepository> _cloudinaryRepository;
    private Mock<IMongoContext> _mongoContext;

    private List<Order> _orders;

    [SetUp]
    public void Setup()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _userRepository = new Mock<IUserRepository>();
        _productRepository = new Mock<IProductRepository>();
        _cloudinaryRepository = new Mock<ICloudinaryRepository>();
        _mongoContext = new Mock<IMongoContext>();

        _orderService = new OrderService(
            _orderRepository.Object,
            _userRepository.Object,
            _productRepository.Object,
            _cloudinaryRepository.Object,
            _mongoContext.Object
        );

        _orders = CreateOrders(2);
    }

    [Test]
    public async Task GetAllAsync_ValidPage_ReturnsPageOfOrders()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 2;
        const int totalItems = 10;

        var pageOfOrders = new PageOf<Order>(_orders, page, pageSize, totalItems);

        _orderRepository
            .Setup(r => r.GetOrders(page, pageSize))
            .ReturnsAsync(pageOfOrders);

        // Act
        var result = await _orderService.GetAllAsync(page, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);

            Assert.That(result.Value.Page, Is.EqualTo(page));

            Assert.That(result.Value.PageSize, Is.EqualTo(pageSize));

            Assert.That(result.Value.TotalItems, Is.EqualTo(totalItems));

            Assert.That(result.Value.Items.Count, Is.EqualTo(_orders.Count));
        });

        _orderRepository.Verify(
            r => r.GetOrders(page, pageSize),
            Times.Once);
    }

    [Test]
    public async Task GetAllAsync_NoOrders_ReturnsEmptyPage()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 10;

        var pageOfOrders = new PageOf<Order>(
            new List<Order>(),
            page,
            pageSize,
            0);

        _orderRepository
            .Setup(r => r.GetOrders(page, pageSize))
            .ReturnsAsync(pageOfOrders);

        // Act
        var result = await _orderService.GetAllAsync(page, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);

            Assert.That(result.Value.Items, Is.Empty);

            Assert.That(result.Value.TotalItems, Is.EqualTo(0));
        });

        _orderRepository.Verify(
            r => r.GetOrders(page, pageSize),
            Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ExistingOrder_ReturnsOrder()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();

        var request = new IdRequest
        {
            Id = orderId.ToString()
        };
        var order = CreateOrder(orderId);

        _orderRepository
            .Setup(r => r.GetOrderById(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);

            Assert.That(result.Value.Id,
                Is.EqualTo(orderId));
        });

        _orderRepository.Verify(
            r => r.GetOrderById(orderId),
            Times.Once);
    }

    private List<Order> CreateOrders(int count = 1)
    {
        var result = new List<Order>();
        for (int i = 0; i < count; i++)
        {
            result.Add(CreateOrder());
        }

        return result;
    }

    private static Order CreateOrder(ObjectId? id = null)
    {
        return new Order
        {
            Id = id ?? ObjectId.GenerateNewId(),
            UserId = ObjectId.GenerateNewId(),
            Address = new Address
            {
                City = "City",
                Street = "Street",
                HouseNumber = "HouseNumber",
            },
            Items = [],
            Price = 100,
            UserFullNameAtOrder = "UserFullNameAtOrder",
            PlacedAt = DateTime.Now,
        };
    }

    [Test]
    public async Task GetByIdAsync_OrderDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();

        var request = new IdRequest
        {
            Id = orderId.ToString()
        };

        _orderRepository
            .Setup(r => r.GetOrderById(orderId))
            .ReturnsAsync(null as Order);

        // Act
        var result = await _orderService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);

            Assert.That(result.Error,
                Is.EqualTo(OrderErrors.NotFound(orderId)));
        });

        _orderRepository.Verify(
            r => r.GetOrderById(orderId),
            Times.Once);
    }
}