using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.ResultPattern;
using MongoDB.Bson;

namespace EMerx.Services.Products;

public interface IProductService
{
    Task<Result<PageOfResponse<ProductResponse>>> GetAllAsync(int page, int pageSize);

    Task<Result<ProductResponse>> GetByIdAsync(IdRequest request);

    Task<Result<ProductResponse>> CreateAsync(ProductRequest request);

    Task<Result> PatchAsync(IdRequest idRequest, PatchProductRequest request);

    Task<Result> DeleteAsync(IdRequest request);
}