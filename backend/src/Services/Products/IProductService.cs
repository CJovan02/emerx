using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Products;

public interface IProductService
{
    Task<Result<IEnumerable<ProductResponse>>> GetAllProductsAsync();
    
    Task<Result<ProductResponse>> GetProductByIdAsync(IdRequest request);
    
    Task<Result<ProductResponse>> CreateProductAsync(ProductRequest request);

    Task<Result> DeleteProductAsync(IdRequest request);
}