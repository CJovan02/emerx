using System.Security.Claims;
using EMerx.Common.Filters;
using EMerx.Controllers;
using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Reviews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests;

public class ReviewControllerTest
{
    private ReviewController _reviewController;
    private Mock<IReviewService> _reviewService;

    private List<ReviewResponse> _reviewResponses;

    private const string _authorizedUserEmail = "authorized@email.com";
    private const string _firebaseUid = "firebase-uid";
    private DefaultHttpContext _emptyHttpContext;
    private DefaultHttpContext _httpContextWithUser;

    [SetUp]
    public void SetUp()
    {
        _reviewService = new Mock<IReviewService>();
        _reviewController = new ReviewController(_reviewService.Object);

        _reviewResponses = CreateReviewResponseList(3);

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

        var pageParams = new PageParams()
        {
            Page = 1,
            PageSize = 10
        };

        var response = new PageOfResponse<ReviewResponse>(
            _reviewResponses,
            1,
            10,
            _reviewResponses.Count);

        _reviewService
            .Setup(s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize))
            .ReturnsAsync(Result<PageOfResponse<ReviewResponse>>.Success(response));

        // Act

        var result = await _reviewController.GetAll(pageParams);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _reviewService.Verify(
            s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize),
            Times.Once);
    }

    [Test]
    public async Task GetById_ExistingReview_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest()
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var response = CreateReviewResponse();

        _reviewService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<ReviewResponse>.Success(response));

        // Act

        var result = await _reviewController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _reviewService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GetById_ReviewNotFound_ReturnsNotFound()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();
        var request = new IdRequest
        {
            Id = objectId.ToString()
        };

        _reviewService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<ReviewResponse>.Failure(
                ReviewErrors.NotFound(objectId)));

        // Act

        var result = await _reviewController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _reviewService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GetByProductId_ValidProductId_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _reviewService
            .Setup(s => s.GetByProductIdAsync(request))
            .ReturnsAsync(Result<IEnumerable<ReviewResponse>>.Success(_reviewResponses));

        // Act

        var result = await _reviewController.GetByProductId(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _reviewService.Verify(
            s => s.GetByProductIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GetByProductId_UnknownProductId_ReturnsNotFound()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();
        var request = new IdRequest
        {
            Id = objectId.ToString()
        };

        _reviewService
            .Setup(s => s.GetByProductIdAsync(request))
            .ReturnsAsync(Result<IEnumerable<ReviewResponse>>.Failure(ReviewErrors.NotFound(objectId)));

        // Act

        var result = await _reviewController.GetByProductId(request);

        // Assert

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

        _reviewService.Verify(
            s => s.GetByProductIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Create_UserAuthenticated_ReturnsCreated()
    {
        // Arrange

        _reviewController.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextWithUser
        };

        var request = new ReviewRequest
        {
            Rating = 5,
            Description = "Great Product!",
            ProductId =  "productId",
        };

        var response = CreateReviewResponse();

        _reviewService
            .Setup(s => s.CreateAsync(_firebaseUid, request))
            .ReturnsAsync(Result<ReviewResponse>.Success(response));

        // Act

        var result = await _reviewController.Create(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _reviewService.Verify(
            s => s.CreateAsync(_firebaseUid, request),
            Times.Once);
    }

    [Test]
    public async Task Create_UserUnauthorized_ReturnsUnauthorized()
    {
        // Arrange

        _reviewController.ControllerContext = new ControllerContext
        {
            HttpContext = _emptyHttpContext
        };

        var request = new ReviewRequest
        {
            Rating = 5,
            Description = "Great product!",
            ProductId = "productId",
        };

        // Act

        var result = await _reviewController.Create(request);

        // Assert

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        _reviewService.Verify(
            s => s.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<ReviewRequest>()),
            Times.Never);
    }

    [Test]
    public async Task Patch_UserAuthenticated_ReturnsOk()
    {
        // Arrange

        _reviewController.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextWithUser
        };

        var idRequest = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var patchRequest = new PatchReviewRequest
        {
            Rating = 4,
            Description = "Updated review",
        };

        var response = CreateReviewResponse();

        _reviewService
            .Setup(s => s.PatchAsync(
                idRequest,
                patchRequest,
                _firebaseUid))
            .ReturnsAsync(Result<ReviewResponse>.Success(response));

        // Act

        var result = await _reviewController.Patch(idRequest, patchRequest);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _reviewService.Verify(
            s => s.PatchAsync(
                idRequest,
                patchRequest,
                _firebaseUid),
            Times.Once);
    }

    [Test]
    public async Task Patch_UserUnauthorized_ReturnsUnauthorized()
    {
        // Arrange

        _reviewController.ControllerContext = new ControllerContext
        {
            HttpContext = _emptyHttpContext
        };

        var idRequest = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var patchRequest = new PatchReviewRequest
        {
            Rating = 4,
            Description = "Updated review"
        };

        // Act

        var result = await _reviewController.Patch(idRequest, patchRequest);

        // Assert

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        _reviewService.Verify(
            s => s.PatchAsync(
                It.IsAny<IdRequest>(),
                It.IsAny<PatchReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public async Task Delete_ExistingReview_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _reviewService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Success());

        // Act

        var result = await _reviewController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _reviewService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Delete_NonExistingReview_ReturnsNotFound()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _reviewService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Failure(ReviewErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _reviewController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

        _reviewService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }


    private List<ReviewResponse> CreateReviewResponseList(int count)
    {
        var list = new List<ReviewResponse>();

        for (var i = 0; i < count; i++)
        {
            list.Add(CreateReviewResponse());
        }

        return list;
    }

    private ReviewResponse CreateReviewResponse()
    {
        return new ReviewResponse
        {
            Id = "id",
            Description = "description",
            ProductId = "productId",
            Rating = 5,
            UserFullName = "userFullName",
            UserId = "userId"
        };
    }
}