using CloudinaryDotNet.Actions;
using EMerx.Common;
using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.Entities;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Services.Products;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Emerx.Tests;

public class ProductServiceTests
{
    private ProductService _sut;
    private Mock<IProductRepository> _productRepository;
    private Mock<ICloudinaryRepository> _cloudinaryRepository;

    private IEnumerable<string> _categories;
    private ObjectId _id;
    private Product _product;

    [SetUp]
    public void Setup()
    {
        _productRepository = new Mock<IProductRepository>();
        _cloudinaryRepository = new Mock<ICloudinaryRepository>();

        _categories = new List<string>
        {
            "category1", "category2", "category3", "category4", "category5", "category6",
            "category7"
        };

        _id = ObjectId.GenerateNewId();
        _product = CreateProduct(_id);

        _productRepository
            .Setup(r => r.GetDistinctCategories())
            .ReturnsAsync(_categories);

        _productRepository
            .Setup(r => r.GetProductById(_id, null))
            .ReturnsAsync(_product);

        _sut = new ProductService(
            _productRepository.Object,
            _cloudinaryRepository.Object);
    }

    [Test]
    public async Task GetCategoriesAsync_ReturnsListOfCategories()
    {
        // Act
        var result = await _sut.GetCategoriesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(_categories.Count()));
        });
        
        _productRepository
            .Verify(r => r.GetDistinctCategories(), Times.Once);
    }

    [Test]
    public async Task GetCategoriesAsync_ReturnsNoListOfCategories()
    {
        // Arrange
        _productRepository
            .Setup(r => r.GetDistinctCategories())
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _sut.GetCategoriesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        });
        
        _productRepository
            .Verify(r => r.GetDistinctCategories(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsSuccess_WhenProductFound_WithNoImage()
    {
        // Arrange
        var request = new IdRequest { Id = _id.ToString() };

        // Act
        var result = await _sut.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Id, Is.EqualTo(_id.ToString()));
            Assert.That(result.Value.ThumbnailUrl, Is.Null);
        });
        
        _productRepository
            .Verify(r => r.GetProductById(_id, null), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.BuildProductThumbnailImageUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsSuccess_WhenProductFound_WithImage()
    {
        //Arrange
        _product.ImageVersion = "v123";
        var expectedUrl = "https://cloudinary.com/image.jpg";
        
        _cloudinaryRepository
            .Setup(c => c.BuildProductThumbnailImageUrl(_id.ToString(), "v123"))
            .Returns(expectedUrl);
        
        var request = new IdRequest { Id = _id.ToString() };

        // Act
        var result = await _sut.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.ThumbnailUrl, Is.EqualTo(expectedUrl));
        });
        
        _productRepository
            .Verify(r => r.GetProductById(_id, null), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.BuildProductThumbnailImageUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange 
        var unknownId = ObjectId.GenerateNewId();
        _productRepository
            .Setup(r => r.GetProductById(unknownId, It.IsAny<IClientSessionHandle?>()))
            .ReturnsAsync((Product?)null);
        
        var request = new IdRequest { Id = unknownId.ToString() };

        // Act
        var result = await _sut.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            
            Assert.That(result.Value, Is.Null);
        });
        
        _productRepository
            .Verify(r => r.GetProductById(unknownId, null), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.BuildProductThumbnailImageUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsSuccess_WithNoImage()
    {
        // Arrange
        var request = CreateProductRequest();

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Name, Is.EqualTo(request.Name));
            Assert.That(result.Value.ThumbnailUrl, Is.Null);
        });

        _productRepository
            .Verify(r => r.CreateProduct(It.IsAny<Product>()), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.UploadProductThumbnailAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ReturnsSuccess_WithImage()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var request = CreateProductRequest(mockFile.Object);

        var uploadResult = new ImageUploadResult { Version = "v123", PublicId = "products/abc/thumbnail" };
        var expectedUrl = "https://cloudinary.com/image.jpg";

        _cloudinaryRepository
            .Setup(c => c.UploadProductThumbnailAsync(It.IsAny<string>(), It.IsAny<Stream>(), false))
            .ReturnsAsync(uploadResult);
        _cloudinaryRepository
            .Setup(c => c.BuildImageUrl(uploadResult.PublicId, uploadResult.Version))
            .Returns(expectedUrl);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Name, Is.EqualTo(request.Name));
            Assert.That(result.Value.ThumbnailUrl, Is.EqualTo(expectedUrl));
        });

        _productRepository
            .Verify(r => r.CreateProduct(It.IsAny<Product>()), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.UploadProductThumbnailAsync(It.IsAny<string>(), It.IsAny<Stream>(), false), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.BuildImageUrl(uploadResult.PublicId, uploadResult.Version), Times.Once);
    }

    [Test]
    public async Task CreateAsync_DoesNotUploadImage_WhenImageIsEmpty()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);

        var request = CreateProductRequest(mockFile.Object);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.ThumbnailUrl, Is.Null);
        });

        _productRepository
            .Verify(r => r.CreateProduct(It.IsAny<Product>()), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.UploadProductThumbnailAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange
        var unknownId = ObjectId.GenerateNewId();
        _productRepository
            .Setup(r => r.GetProductById(unknownId, null))
            .ReturnsAsync((Product?)null);

        var idRequest = new IdRequest { Id = unknownId.ToString() };
        var request = CreatePatchRequest();

        // Act
        var result = await _sut.PatchAsync(idRequest, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
        });

        _productRepository
            .Verify(r => r.UpdateProduct(It.IsAny<ObjectId>(), It.IsAny<UpdateDefinition<Product>>(), null), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsFailure_WhenNoUpdatesProvided()
    {
        // Arrange
        var idRequest = new IdRequest { Id = _id.ToString() };
        var request = CreatePatchRequest();

        // Act
        var result = await _sut.PatchAsync(idRequest, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
        });

        _productRepository
            .Verify(r => r.UpdateProduct(It.IsAny<ObjectId>(), It.IsAny<UpdateDefinition<Product>>(), null), Times.Never);
    }

    [Test]
    public async Task PatchAsync_ReturnsSuccess_WhenFieldsAreUpdated()
    {
        // Arrange
        var idRequest = new IdRequest { Id = _id.ToString() };
        var request = CreatePatchRequest(name: "Updated Name", price: 20m);

        // Act
        var result = await _sut.PatchAsync(idRequest, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _productRepository
            .Verify(r => r.UpdateProduct(_id, It.IsAny<UpdateDefinition<Product>>(), null), Times.Once);
    }

    [Test]
    public async Task PatchAsync_DeletesImage_WhenImageIsDeleteOperation()
    {
        // Arrange
        _cloudinaryRepository
            .Setup(c => c.DeleteProductThumbnail(_id.ToString()))
            .ReturnsAsync(new DeletionResult());

        var idRequest = new IdRequest { Id = _id.ToString() };
        var request = CreatePatchRequest(image: new OptionalField<IFormFile?>(null));

        // Act
        var result = await _sut.PatchAsync(idRequest, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _cloudinaryRepository
            .Verify(c => c.DeleteProductThumbnail(_id.ToString()), Times.Once);
        _productRepository
            .Verify(r => r.UpdateProduct(_id, It.IsAny<UpdateDefinition<Product>>(), null), Times.Once);
    }

    [Test]
    public async Task PatchAsync_ReplacesImage_WhenImageIsReplaceOperation()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        _cloudinaryRepository
            .Setup(c => c.UploadProductThumbnailAsync(_id.ToString(), It.IsAny<Stream>(), true))
            .ReturnsAsync(new ImageUploadResult { Version = "v456", PublicId = "products/abc/thumbnail" });

        var idRequest = new IdRequest { Id = _id.ToString() };
        var request = CreatePatchRequest(image: new OptionalField<IFormFile?>(mockFile.Object));

        // Act
        var result = await _sut.PatchAsync(idRequest, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _cloudinaryRepository
            .Verify(c => c.UploadProductThumbnailAsync(_id.ToString(), It.IsAny<Stream>(), true), Times.Once);
        _productRepository
            .Verify(r => r.UpdateProduct(_id, It.IsAny<UpdateDefinition<Product>>(), null), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange
        var unknownId = ObjectId.GenerateNewId();
        _productRepository
            .Setup(r => r.GetProductById(unknownId, null))
            .ReturnsAsync((Product?)null);

        var request = new IdRequest { Id = unknownId.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);

        _cloudinaryRepository
            .Verify(c => c.DeleteProductImages(It.IsAny<string>()), Times.Never);
        _cloudinaryRepository
            .Verify(c => c.DeleteProductFolder(It.IsAny<string>()), Times.Never);
        _productRepository
            .Verify(r => r.DeleteProduct(It.IsAny<ObjectId>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ReturnsSuccess_WhenProductHasNoImage()
    {
        // Arrange
        _cloudinaryRepository
            .Setup(c => c.DeleteProductImages(_id.ToString()))
            .ReturnsAsync(new DelResResult());

        var request = new IdRequest { Id = _id.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _cloudinaryRepository
            .Verify(c => c.DeleteProductImages(_id.ToString()), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.DeleteProductFolder(It.IsAny<string>()), Times.Never);
        _productRepository
            .Verify(r => r.DeleteProduct(_id), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ReturnsSuccess_AndDeletesFolder_WhenProductHasImage()
    {
        // Arrange
        _product.ImageVersion = "v123";

        _cloudinaryRepository
            .Setup(c => c.DeleteProductImages(_id.ToString()))
            .ReturnsAsync(new DelResResult());
        _cloudinaryRepository
            .Setup(c => c.DeleteProductFolder(_id.ToString()))
            .ReturnsAsync(new DeleteFolderResult());

        var request = new IdRequest { Id = _id.ToString() };

        // Act
        var result = await _sut.DeleteAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _cloudinaryRepository
            .Verify(c => c.DeleteProductImages(_id.ToString()), Times.Once);
        _cloudinaryRepository
            .Verify(c => c.DeleteProductFolder(_id.ToString()), Times.Once);
        _productRepository
            .Verify(r => r.DeleteProduct(_id), Times.Once);
    }

    private static CreateProductRequest CreateProductRequest(IFormFile? image = null) =>
        new()
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10m,
            Stock = 5,
            Image = image
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

    private static PatchProductRequest CreatePatchRequest(
        OptionalField<IFormFile?>? image = null,
        string? name = null,
        string? description = null,
        string? category = null,
        decimal? price = null,
        int? stock = null) =>
        new()
        {
            Image = image ?? new OptionalField<IFormFile?>(),
            Name = name,
            Description = description,
            Category = category,
            Price = price,
            Stock = stock
        };
}
