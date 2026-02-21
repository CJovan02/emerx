using EMerx.Common.Filters;
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
    public async Task<Result<PageOfResponse<ProductResponse>>> GetAllAsync(int page, int pageSize)
    {
        var pageOfProducts = await productRepository.GetProducts(page, pageSize);
        var productResponses = pageOfProducts
            .Items
            .Select(product => product.ToResponse())
            .ToList();

        var response = new PageOfResponse<ProductResponse>(
            productResponses,
            pageOfProducts.Page,
            pageOfProducts.PageSize,
            pageOfProducts.TotalItems);
        
        return Result<PageOfResponse<ProductResponse>>.Success(response);
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