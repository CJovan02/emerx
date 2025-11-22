using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.ResultPattern;

namespace EMerx.Services.Products;

public interface IProductService
{
    Task<Result<IEnumerable<ProductResponse>>> GetAllAsync();

    Task<Result<ProductResponse>> GetByIdAsync(IdRequest request);

    Task<Result<ProductResponse>> CreateAsync(ProductRequest request);

    Task<Result> DeleteAsync(IdRequest request);
}