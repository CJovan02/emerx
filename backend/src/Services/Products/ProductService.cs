using EMerx.DTOs.Id;
using EMerx.DTOs.Products;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.Repositories.ProductRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;

namespace EMerx.Services.Products;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<IEnumerable<ProductResponse>>> GetAllAsync()
    {
        var products = await productRepository.GetProducts();
        var productsList = products
            .Select(product => product.ToResponse())
            .ToList();

        return Result<IEnumerable<ProductResponse>>.Success(productsList);
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var product = await productRepository.GetProductById(objectId);
        if (product is null)
        {
            return Result<ProductResponse>.Failure(ProductErrors.NotFound(objectId));
        }
        return Result<ProductResponse>.Success(product.ToResponse());
    }

    public async Task<Result<ProductResponse>> CreateAsync(ProductRequest request)
    {
        var product = request.ToDomain();
        await productRepository.CreateProduct(product);
        return Result<ProductResponse>.Success(product.ToResponse());
    }

    public async Task<Result> DeleteAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        await productRepository.DeleteProduct(objectId);
        return Result.Success();
    }
}