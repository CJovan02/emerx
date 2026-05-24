using System.Security.Claims;
using EMerx.Common.Filters;
using EMerx.Controllers;
using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests.Controllers;

public class ProductControllerTests
{
    private ProductController _productController;
    private Mock<IProductService> _productService;

    private const string _authorizedUserEmail = "authorized@email.com";
    private const string _firebaseUid = "firebase-uid";
    private DefaultHttpContext _emptyHttpContext;
    private DefaultHttpContext _httpContextWithUser;

    [SetUp]
    public void SetUp()
    {
        _productService = new Mock<IProductService>();
        _productController =  new ProductController(_productService.Object);

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
    public async Task GetPaged_ValidParams_ReturnsOk()
    {
        // Arrange

        var pageParams = new PageParams()
        {
            Page = 1,
            PageSize = 10
        };

        var filterParams = new ProductFilterParams
        {
            Category = "Electronics"
        };

        var response = new PageOfResponse<ProductResponse>(
            [],
            1,
            10,
            0);

        _productService
            .Setup(s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize,
                filterParams))
            .ReturnsAsync(Result<PageOfResponse<ProductResponse>>.Success(response));

        // Act

        var result = await _productController.GetPaged(pageParams, filterParams);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _productService.Verify(
            s => s.GetAllAsync(
                pageParams.Page,
                pageParams.PageSize,
                filterParams),
            Times.Once);
    }

    [Test]
    public async Task GetCategories_ReturnsOk()
    {
        // Arrange

        var categories = new List<string>
        {
            "Electronics",
            "Books",
            "Clothes"
        };

        _productService
            .Setup(s => s.GetCategoriesAsync())
            .ReturnsAsync(categories);

        // Act

        var result = await _productController.GetCategories();

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _productService.Verify(
            s => s.GetCategoriesAsync(),
            Times.Once);
    }

    [Test]
    public async Task GetById_ExistingProduct_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest()
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var response = CreateProductResponse();

        _productService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<ProductResponse>.Success(response));

        // Act

        var result = await _productController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _productService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GetById_ProductNotFound_ReturnsNotFound()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _productService
            .Setup(s => s.GetByIdAsync(request))
            .ReturnsAsync(Result<ProductResponse>.Failure(
                ProductErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _productController.GetById(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _productService.Verify(
            s => s.GetByIdAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange

        var request = new CreateProductRequest()
        {
            Name = "Laptop",
            Description = "Gaming laptop",
            Category = "Electronics",
            Price = 1500,
            Stock = 5
        };

        var response = CreateProductResponse();

        _productService
            .Setup(s => s.CreateAsync(request))
            .ReturnsAsync(Result<ProductResponse>.Success(response));

        // Act

        var result = await _productController.Create(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _productService.Verify(
            s => s.CreateAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Patch_ExistingProduct_ReturnsOk()
    {
        // Arrange

        var idRequest = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var request = new PatchProductRequest
        {
            Name = "Updated Laptop",
            Price = 2000,
            Image = null,
            Category = "Electronics",
            Description = "Description",
            Stock = 5
        };

        var response = CreateProductResponse();

        _productService
            .Setup(s => s.PatchAsync(idRequest, request))
            .ReturnsAsync(Result<ProductResponse>.Success(response));

        // Act

        var result = await _productController.Patch(idRequest, request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _productService.Verify(
            s => s.PatchAsync(idRequest, request),
            Times.Once);
    }

    [Test]
    public async Task Patch_ProductNotFound_ReturnsNotFound()
    {
        // Arrange

        var idRequest = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        var request = new PatchProductRequest
        {
            Name = "Updated Laptop",
            Price = 2000,
            Image = null,
            Category = "Electronics",
            Description = "Description",
            Stock = 5
        };

        _productService
            .Setup(s => s.PatchAsync(idRequest, request))
            .ReturnsAsync(Result<ProductResponse>.Failure(
                ProductErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _productController.Patch(idRequest, request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _productService.Verify(
            s => s.PatchAsync(idRequest, request),
            Times.Once);
    }

    [Test]
    public async Task Delete_ExistingProduct_ReturnsOk()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _productService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Success());

        // Act

        var result = await _productController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _productService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Delete_ProductNotFound_ReturnsNotFound()
    {
        // Arrange

        var request = new IdRequest
        {
            Id = ObjectId.GenerateNewId().ToString()
        };

        _productService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Failure(
                ProductErrors.NotFound(ObjectId.GenerateNewId())));

        // Act

        var result = await _productController.Delete(request);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _productService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    private ProductResponse CreateProductResponse()
    {
        return new ProductResponse
        {
            Id = "ProductId",
            Name = "Product",
            Description = "Description",
            Category = "Electronics",
            Price = 100,
            Stock = 10,
            AverageRating = 5,
            ReviewCount = 50,
            ThumbnailUrl = "url"
        };
    }

}