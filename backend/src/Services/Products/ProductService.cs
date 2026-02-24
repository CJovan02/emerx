using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Products;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.Entities;
using EMerx.Repositories.ProductRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;
using MongoDB.Driver;

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

    public async Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var product = request.ToDomain();
        await productRepository.CreateProduct(product);
        return Result<ProductResponse>.Success(product.ToResponse());
    }

    public async Task<Result> PatchAsync(IdRequest idRequest, PatchProductRequest request)
    {
        var id = ObjectId.Parse(idRequest.Id);

        if (!await productRepository.ProductExists(id))
            return Result.Failure(ProductErrors.NotFound(id));

        // create update definition to pass to repo layer
        // maybe this will break separation of concerns, since now service layer knows that repo layer
        // uses mongo for db implementation, idk. :)
        var updates = new List<UpdateDefinition<Product>>();
        if (request.Name is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Name, request.Name));
        if (request.Category is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Category, request.Category));
        if (request.Image is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Image, request.Image));
        if (request.Price is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Price, request.Price));

        if (updates.Count == 0)
            return Result.Failure(ProductErrors.NoUpdates(id));

        var updateDef = Builders<Product>.Update.Combine(updates);
        await productRepository.UpdateProduct(id, updateDef);

        return Result.Success();
    }

    // TODO delete all of the reviews for the product as well
    // Orders for this product should not be deleted
    public async Task<Result> DeleteAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        await productRepository.DeleteProduct(objectId);
        return Result.Success();
    }
}