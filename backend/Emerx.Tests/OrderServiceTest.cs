using EMerx.Common.Filters;
using EMerx.DTOs.Address;
using EMerx.DTOs.Id;
using EMerx.DTOs.OrderItems.Request;
using EMerx.DTOs.Orders.Request;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Orders;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Emerx.Tests;

public class OrderServiceTest
{
    private OrderService _orderService;
    private Mock<IOrderRepository> _orderRepository;
    private Mock<IUserRepository> _userRepository;
    private Mock<IProductRepository> _productRepository;
    private Mock<ICloudinaryRepository> _cloudinaryRepository;

    private Mock<IClientSessionHandle> _session;
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
        _session = new Mock<IClientSessionHandle>();
        _mongoContext
            .Setup(c => c.StartSessionAsync())
            .ReturnsAsync(_session.Object);

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

    [Test]
    public async Task CreateAsync_ValidRequest_CreatesOrder()
    {
        // Arrange

        var firebaseUid = "uid123";

        var user = CreateUser();

        var product = CreateProduct();

        var request = CreateOrderRequest(product.Id.ToString());

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(user);

        _productRepository
            .Setup(r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(),
                _session.Object))
            .ReturnsAsync([product]);

        var updateResultMock = new Mock<UpdateResult>();

        updateResultMock
            .Setup(x => x.ModifiedCount)
            .Returns(1);

        _productRepository
            .Setup(r => r.UpdateProduct(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<UpdateDefinition<Product>>(),
                _session.Object))
            .ReturnsAsync(updateResultMock.Object);

        // Act

        var result = await _orderService.CreateAsync(firebaseUid, request);

        // Assert

        Assert.That(result.IsSuccess, Is.True);

        _mongoContext.Verify(
            c => c.StartSessionAsync(),
            Times.Once);

        _userRepository.Verify(
            r => r.GetUserByFirebaseUid(firebaseUid, _session.Object),
            Times.Once);
        _productRepository.Verify(
            r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(),
                _session.Object),
            Times.Once);

        _orderRepository.Verify(
            r => r.CreateOrder(
                It.IsAny<Order>(),
                _session.Object),
            Times.Once);

        _session.Verify(
            s => s.AbortTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
        _session.Verify(
            s => s.CommitTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task CreateAsync_UserDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(
                It.IsAny<string>(),
                _session.Object))
            .ReturnsAsync(null as User);

        // Act

        var result = await _orderService.CreateAsync(
            "uid",
            CreateOrderRequest());

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);
            Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound("uid")));
        });

        _mongoContext.Verify(
            c => c.StartSessionAsync(),
            Times.Once);

        _session.Verify(
            s => s.AbortTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _session.Verify(
            s => s.CommitTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);


        _productRepository.Verify(
            r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(), _session.Object),
            Times.Never);

        _productRepository.Verify(
            r => r.UpdateProduct(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<UpdateDefinition<Product>>(),
                _session.Object),
            Times.Never);

        _orderRepository.Verify(
            r => r.CreateOrder(
                It.IsAny<Order>(),
                It.IsAny<IClientSessionHandle>()),
            Times.Never);
    }

    [Test]
    public async Task CreateAsync_QuantityNotAvailable_ReturnsError()
    {
        // Arrange

        var firebaseUid = "uid123";

        var user = CreateUser();

        var product = CreateProduct();

        var request = CreateOrderRequest(product.Id.ToString());

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(user);

        _productRepository
            .Setup(r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(),
                _session.Object))
            .ReturnsAsync([product]);

        var updateResultMock = new Mock<UpdateResult>();

        updateResultMock
            .Setup(x => x.ModifiedCount)
            .Returns(0);

        _productRepository
            .Setup(r => r.UpdateProduct(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<UpdateDefinition<Product>>(),
                _session.Object))
            .ReturnsAsync(updateResultMock.Object);

        // Act

        var result = await _orderService.CreateAsync(firebaseUid, request);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(
                OrderErrors.QuantityNotAvailable(
                    request.Items.First().ProductId,
                    product.Stock,
                    request.Items.First().Quantity)));
        });

        _mongoContext.Verify(
            c => c.StartSessionAsync(),
            Times.Once);

        _userRepository.Verify(
            r => r.GetUserByFirebaseUid(firebaseUid, _session.Object),
            Times.Once);
        _productRepository.Verify(
            r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(),
                _session.Object),
            Times.Once);

        _orderRepository.Verify(
            r => r.CreateOrder(
                It.IsAny<Order>(),
                _session.Object),
            Times.Once);

        _session.Verify(
            s => s.AbortTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
        _session.Verify(
            s => s.CommitTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetOrderReview_ValidRequest_ReturnsReviewResponse()
    {
        // Arrange
        var product = CreateProduct();

        var request = CreateOrderReviewRequest(product.Id.ToString());


        _productRepository
            .Setup(r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(), null))
            .ReturnsAsync([product]);

        _cloudinaryRepository
            .Setup(r => r.BuildProductThumbnailImageUrl(
                It.IsAny<string>(), null))
            .Returns("thumbnail-url");

        // Act

        var result = await _orderService.GetOrderReview(request);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);

            Assert.That(result.Value.Items.Count, Is.EqualTo(1));

            Assert.That(result.Value.Items[0].Quantity, Is.EqualTo(2));
        });

        _productRepository.Verify(
            r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(), null),
            Times.Once);
    }

    [Test]
    public async Task GetOrderReview_ProductDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var missingProductId = ObjectId.GenerateNewId();
        var request = CreateOrderReviewRequest(missingProductId.ToString());

        _productRepository
            .Setup(r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(), null))
            .ReturnsAsync([]);

        // Act

        var result = await _orderService.GetOrderReview(request);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);

            Assert.That(
                result.Error,
                Is.EqualTo(OrderErrors.NotFound([missingProductId])));
        });
    }

    [Test]
    public async Task GetOrderReview_QuantityNotAvailable_ReturnsError()
    {
        // Arrange
        const int stock = 5;
        const int quantity = 20;
        var product = CreateProduct(stock);
        var request = CreateOrderReviewRequest(product.Id.ToString(), quantity);


        _productRepository
            .Setup(r => r.GetProductsByIds(
                It.IsAny<List<ObjectId>>(), null))
            .ReturnsAsync([product]);

        // Act

        var result = await _orderService.GetOrderReview(request);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);

            Assert.That(
                result.Error,
                Is.EqualTo(
                    OrderErrors.QuantityNotAvailable(
                        product.Id.ToString(),
                        stock,
                        quantity)));
        });
    }

    [Test]
    public async Task DeleteAsync_ExistingOrder_DeletesOrder()
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

        var result = await _orderService.DeleteAsync(request);

        // Assert

        Assert.That(result.IsSuccess, Is.True);

        _orderRepository.Verify(
            r => r.DeleteOrder(orderId),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_OrderDoesNotExist_ReturnsNotFoundError()
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

        var result = await _orderService.DeleteAsync(request);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);

            Assert.That(
                result.Error,
                Is.EqualTo(OrderErrors.NotFound(orderId)));
        });

        _orderRepository.Verify(
            r => r.DeleteOrder(It.IsAny<ObjectId>()),
            Times.Never);
    }

    private OrderReviewRequest CreateOrderReviewRequest(string? productId = null, int quantity = 2)
    {
        return new OrderReviewRequest
        {
            Items =
            [
                new OrderItemRequest
                {
                    ProductId = productId ?? "id123",
                    Quantity = quantity
                }
            ]
        };
    }

    private OrderRequest CreateOrderRequest(string? productId = null)
    {
        return new OrderRequest
        {
            Address = new AddressRequiredDto
            {
                City = "City",
                HouseNumber = "HouseNum",
                Street = "Street"
            },
            Items =
            [
                new OrderItemRequest
                {
                    ProductId = productId ?? "id123",
                    Quantity = 2
                }
            ]
        };
    }

    private Product CreateProduct(int stock = 10)
    {
        return new Product
        {
            Id = ObjectId.GenerateNewId(),
            Name = "Product",
            Price = 10,
            Category = "Category",
            Description = "Description",
            AverageRating = 4,
            ReviewCount = 5,
            SumRatings = 20,
            Stock = stock,
            ImageVersion = null
        };
    }

    private User CreateUser(ObjectId? id = null, string? email = null, string? firebaseUid = null)
    {
        return new User
        {
            Email = email ?? "johnpeacock@email.com",
            Id = id ?? ObjectId.GenerateNewId(),
            Name = "John",
            Surname = "Peacock",
            FirebaseUid = firebaseUid,
        };
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
}