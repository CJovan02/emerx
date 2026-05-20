using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews.Request;
using EMerx.Entities;
using EMerx.Infrastructure.MongoDb;
using EMerx.Repositories.OrderRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Repositories.ReviewRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Reviews;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Emerx.Tests;

public class ReviewServiceTests
{
    private ReviewService _sut;
    private Mock<IUserRepository> _userRepository;
    private Mock<IReviewRepository> _reviewRepository;
    private Mock<IProductRepository> _productRepository;
    private Mock<IOrderRepository> _orderRepository;
    private Mock<IMongoContext> _mongoContext;
    private Mock<IClientSessionHandle> _session;

    private ObjectId _id;
    private IEnumerable<Review> _reviews;

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<IUserRepository>();
        _reviewRepository = new Mock<IReviewRepository>();
        _productRepository = new Mock<IProductRepository>();
        _orderRepository = new Mock<IOrderRepository>();
        _mongoContext = new Mock<IMongoContext>();
        _session = new Mock<IClientSessionHandle>();

        _mongoContext
            .Setup(c => c.StartSessionAsync())
            .ReturnsAsync(_session.Object);

        _reviews = new List<Review>
        {
            CreateReview(_id),
            CreateReview(_id),
        };

        _reviewRepository
            .Setup(r => r.GetReviewsForProduct(It.IsAny<ObjectId>()))
            .ReturnsAsync(_reviews);

        _sut = new ReviewService(
            _userRepository.Object,
            _reviewRepository.Object,
            _productRepository.Object,
            _orderRepository.Object,
            _mongoContext.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsSuccess_WithEmptyPage()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var pagedResult = new PageOf<Review>([], page, pageSize, 0);
        _reviewRepository
            .Setup(r => r.GetReviews(page, pageSize))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetAllAsync(page, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Items, Is.Empty);
            Assert.That(result.Value.TotalItems, Is.EqualTo(0));
        });

        _reviewRepository
            .Verify(r => r.GetReviews(page, pageSize), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ReturnsSuccess_WithReviews()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var productId = ObjectId.GenerateNewId();
        var reviews = new List<Review> { CreateReview(productId), CreateReview(productId) };
        var pagedResult = new PageOf<Review>(reviews, page, pageSize, reviews.Count);
        _reviewRepository
            .Setup(r => r.GetReviews(page, pageSize))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetAllAsync(page, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Items.Count(), Is.EqualTo(reviews.Count));
        });

        _reviewRepository
            .Verify(r => r.GetReviews(page, pageSize), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_PreservesPaginationMetadata()
    {
        // Arrange
        var page = 2;
        var pageSize = 5;
        var pagedResult = new PageOf<Review>([], page, pageSize, 20);
        _reviewRepository
            .Setup(r => r.GetReviews(page, pageSize))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetAllAsync(page, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Page, Is.EqualTo(page));
            Assert.That(result.Value.PageSize, Is.EqualTo(pageSize));
            Assert.That(result.Value.TotalItems, Is.EqualTo(20));
        });

        _reviewRepository
            .Verify(r => r.GetReviews(page, pageSize), Times.Once);
    }

    [Test]
    public async Task GetByProductIdAsync_ReturnsSuccess_WhenProductHasReviews()
    {
        var request = new IdRequest { Id = _id.ToString() };

        // Act
        var result = await _sut.GetByProductIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Count(), Is.EqualTo(_reviews.Count()));
        });

        _reviewRepository
            .Verify(r => r.GetReviewsForProduct(_id), Times.Once);
    }

    [Test]
    public async Task GetByProductIdAsync_ReturnsSuccess_WithEmptyList_WhenProductHasNoReviews()
    {
        // Arrange
        var productId = ObjectId.GenerateNewId();

        _reviewRepository
            .Setup(r => r.GetReviewsForProduct(productId))
            .ReturnsAsync(new List<Review>());

        var request = new IdRequest { Id = productId.ToString() };

        // Act
        var result = await _sut.GetByProductIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        });

        _reviewRepository
            .Verify(r => r.GetReviewsForProduct(productId), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ReturnsSuccess_WhenAllConditionsMet()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var user = CreateUser(userId);
        var product = CreateProduct(productId);
        var request = CreateReviewRequest(productId);

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(user);
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync(product);
        _orderRepository
            .Setup(r => r.HasUserOrderedProduct(userId, productId, _session.Object))
            .ReturnsAsync(true);
        _reviewRepository
            .Setup(r => r.UserPostedReviewForProduct(userId, productId, _session.Object))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(firebaseUid, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.ProductId, Is.EqualTo(productId.ToString()));
        });

        _userRepository
            .Verify(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object), Times.Once);
        _productRepository
            .Verify(r => r.GetProductById(productId, _session.Object), Times.Once);
        _orderRepository
            .Verify(r => r.HasUserOrderedProduct(userId, productId, _session.Object), Times.Once);
        _reviewRepository
            .Verify(r => r.UserPostedReviewForProduct(userId, productId, _session.Object), Times.Once);
        _productRepository
            .Verify(r => r.UpdateProductReviewAsync(productId, It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), _session.Object), Times.Once);
        _reviewRepository
            .Verify(r => r.CreateReview(It.IsAny<Review>(), _session.Object), Times.Once);
        _session
            .Verify(s => s.CommitTransactionAsync(default), Times.Once);
        _session
            .Verify(s => s.AbortTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var firebaseUid = "unknown-uid";

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync((User?)null);

        var request = CreateReviewRequest(ObjectId.GenerateNewId());

        // Act
        var result = await _sut.CreateAsync(firebaseUid, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound(firebaseUid)));
        });

        _productRepository.Verify(r => r.GetProductById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _orderRepository.Verify(r => r.HasUserOrderedProduct(It.IsAny<ObjectId>(), It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UserPostedReviewForProduct(It.IsAny<ObjectId>(), It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.CreateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync((Product?)null);

        var request = CreateReviewRequest(productId);

        // Act
        var result = await _sut.CreateAsync(firebaseUid, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ProductErrors.NotFound(productId)));
        });

        _orderRepository.Verify(r => r.HasUserOrderedProduct(It.IsAny<ObjectId>(), It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UserPostedReviewForProduct(It.IsAny<ObjectId>(), It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.CreateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsFailure_WhenUserHasNotOrderedProduct()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync(CreateProduct(productId));
        _orderRepository
            .Setup(r => r.HasUserOrderedProduct(userId, productId, _session.Object))
            .ReturnsAsync(false);

        var request = CreateReviewRequest(productId);

        // Act
        var result = await _sut.CreateAsync(firebaseUid, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.NotOrdered(userId, productId)));
        });

        _reviewRepository.Verify(r => r.UserPostedReviewForProduct(It.IsAny<ObjectId>(), It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.CreateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsFailure_WhenUserAlreadyReviewedProduct()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync(CreateProduct(productId));
        _orderRepository
            .Setup(r => r.HasUserOrderedProduct(userId, productId, _session.Object))
            .ReturnsAsync(true);
        _reviewRepository
            .Setup(r => r.UserPostedReviewForProduct(userId, productId, _session.Object))
            .ReturnsAsync(true);

        var request = CreateReviewRequest(productId);

        // Act
        var result = await _sut.CreateAsync(firebaseUid, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.UserPostedReviewForProduct(userId, productId)));
        });

        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.CreateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenNoUpdatesProvided()
    {
        // Arrange
        var reviewId = ObjectId.GenerateNewId();
        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest();

        // Act
        var result = await _sut.PatchAsync(idRequest, request, "firebase-uid");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.NoUpdates(reviewId)));
        });

        _mongoContext.Verify(c => c.StartSessionAsync(), Times.Never);
        _userRepository.Verify(r => r.GetUserByFirebaseUid(It.IsAny<string>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.GetReviewById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var firebaseUid = "unknown-uid";
        var reviewId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync((User?)null);

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(description: "Updated");

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound(firebaseUid)));
        });

        _reviewRepository.Verify(r => r.GetReviewById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenReviewNotFound()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var reviewId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync((Review?)null);

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(description: "Updated");

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.NotFound(reviewId)));
        });

        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenReviewDoesNotBelongToUser()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(CreateReview(productId, ObjectId.GenerateNewId())); // different userId

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(description: "Updated");

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.NotYourReview()));
        });

        _productRepository.Verify(r => r.GetProductById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenProductNotFound_DuringRatingUpdate()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var review = CreateReview(productId, userId);

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(review);
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync((Product?)null);

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(rating: 3.0); // different from review's 4.5

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ProductErrors.NotFound(productId)));
        });

        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsSuccess_WhenDescriptionUpdated()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var review = CreateReview(productId, userId);

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(review);

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(description: "Updated description");

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _productRepository.Verify(r => r.GetProductById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), _session.Object), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Once);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsSuccess_WhenRatingUpdated()
    {
        // Arrange
        var firebaseUid = "firebase-uid";
        var userId = ObjectId.GenerateNewId();
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var review = CreateReview(productId, userId);

        _userRepository
            .Setup(r => r.GetUserByFirebaseUid(firebaseUid, _session.Object))
            .ReturnsAsync(CreateUser(userId));
        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(review);
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync(CreateProduct(productId));

        var idRequest = new IdRequest { Id = reviewId.ToString() };
        var request = CreatePatchRequest(rating: 3.0); // different from review's 4.5

        // Act
        var result = await _sut.PatchAsync(idRequest, request, firebaseUid);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _productRepository.Verify(r => r.GetProductById(productId, _session.Object), Times.Once);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(productId, It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), _session.Object), Times.Once);
        _reviewRepository.Verify(r => r.UpdateReview(It.IsAny<Review>(), _session.Object), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Once);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFailure_WhenReviewNotFound()
    {
        // Arrange
        var reviewId = ObjectId.GenerateNewId();

        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync((Review?)null);

        var request = new IdRequest { Id = reviewId.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ReviewErrors.NotFound(reviewId)));
        });

        _reviewRepository.Verify(r => r.DeleteReview(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.GetProductById(It.IsAny<ObjectId>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var review = CreateReview(productId, ObjectId.GenerateNewId());

        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(review);
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync((Product?)null);

        var request = new IdRequest { Id = reviewId.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(ProductErrors.NotFound(productId)));
        });

        _reviewRepository.Verify(r => r.DeleteReview(review.Id, _session.Object), Times.Once);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(It.IsAny<ObjectId>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<IClientSessionHandle>()), Times.Never);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ReturnsSuccess()
    {
        // Arrange
        var reviewId = ObjectId.GenerateNewId();
        var productId = ObjectId.GenerateNewId();
        var review = CreateReview(productId, ObjectId.GenerateNewId());

        _reviewRepository
            .Setup(r => r.GetReviewById(reviewId, _session.Object))
            .ReturnsAsync(review);
        _productRepository
            .Setup(r => r.GetProductById(productId, _session.Object))
            .ReturnsAsync(CreateProduct(productId));

        var request = new IdRequest { Id = reviewId.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _reviewRepository.Verify(r => r.DeleteReview(review.Id, _session.Object), Times.Once);
        _productRepository.Verify(r => r.GetProductById(productId, _session.Object), Times.Once);
        _productRepository.Verify(r => r.UpdateProductReviewAsync(productId, It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), _session.Object), Times.Once);
        _session.Verify(s => s.CommitTransactionAsync(default), Times.Once);
        _session.Verify(s => s.AbortTransactionAsync(default), Times.Never);
    }

    private static Review CreateReview(ObjectId productId, ObjectId? userId = null) =>
        new()
        {
            UserId = userId ?? ObjectId.GenerateNewId(),
            UserFullName = "Test User",
            ProductId = productId,
            Rating = 4.5,
            Description = "Test Description"
        };

    private static User CreateUser(ObjectId id) =>
        new()
        {
            Id = id,
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com"
        };

    private static Product CreateProduct(ObjectId id) =>
        new()
        {
            Id = id,
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            ImageVersion = null,
            Price = 10m
        };

    private static ReviewRequest CreateReviewRequest(ObjectId productId) =>
        new()
        {
            ProductId = productId.ToString(),
            Rating = 4.5,
            Description = "Great product!"
        };

    private static PatchReviewRequest CreatePatchRequest(double? rating = null, string? description = null) =>
        new()
        {
            Rating = rating,
            Description = description
        };
}
