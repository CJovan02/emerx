using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.Entities;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.Services.Products;
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
}
