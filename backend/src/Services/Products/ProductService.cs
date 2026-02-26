using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Products;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.Entities;
using EMerx.Repositories.CloudinaryRepository;
using EMerx.Repositories.ProductRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx.Services.Products;

public class ProductService(
    IProductRepository productRepository,
    ICloudinaryRepository cloudinaryRepository,
    ILogger<ProductService> logger)
    : IProductService
{
    public async Task<Result<PageOfResponse<ProductResponse>>> GetAllAsync(int page, int pageSize)
    {
        var pageOfProducts = await productRepository.GetProducts(page, pageSize);
        var productResponses = pageOfProducts
            .Items
            .Select(product =>
            {
                string imgUrl = null;
                // This check is the main reason why we store HasImage in the db
                if (product.HasImage)
                    imgUrl = cloudinaryRepository.BuildProductThumbnailImageUrl(product.Id.ToString());

                return product.ToResponse(imgUrl);
            })
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
        // I manually generate object id since uploading image requires it
        // I could also create a product first, use that id to upload image and then update the product with the new image
        // But this approach saves us one database call
        var productId = ObjectId.GenerateNewId();

        var hasImage = request.Image is not null && request.Image.Length != 0;
        string imageUrl = null;
        if (hasImage)
        {
            await using var stream = request.Image.OpenReadStream();
            var imageResult =
                await cloudinaryRepository.UploadProductThumbnailAsync(productId.ToString(), stream);

            imageUrl = cloudinaryRepository.BuildImageUrl(imageResult.PublicId);
        }

        var product = request.ToDomain(productId, hasImage);
        await productRepository.CreateProduct(product);

        return Result<ProductResponse>.Success(product.ToResponse(imageUrl));
    }

    public async Task<Result> PatchAsync(IdRequest idRequest, PatchProductRequest request)
    {
        var id = ObjectId.Parse(idRequest.Id);

        var product = await  productRepository.GetProductById(id);
        if (product is null)
            return Result.Failure(ProductErrors.NotFound(id));

        var isReplace = request.Image.IsReplaceOperation();
        var isDelete = request.Image.IsDeleteOperation();
        var isNothing = request.Image.IsNothing();

        if (isDelete)
        {
            await cloudinaryRepository.DeleteProductThumbnail(id.ToString());
        }

        if (isReplace)
        {
            await using var stream = request.Image.Value!.OpenReadStream();
            // set overwrite to true to overwrite the original image
            await cloudinaryRepository.UploadProductThumbnailAsync(id.ToString(), stream, overwrite: true);
        }

        var updates = new List<UpdateDefinition<Product>>();
        if (isDelete)
            updates.Add(Builders<Product>.Update.Set(x => x.HasImage, false));
        if (isReplace && !product.HasImage)
            updates.Add(Builders<Product>.Update.Set(x => x.HasImage, true));
        if (request.Name is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Name, request.Name));
        if (request.Category is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Category, request.Category));
        if (request.Price is not null)
            updates.Add(Builders<Product>.Update.Set(x => x.Price, request.Price));

        if (updates.Count == 0 && isNothing)
            return Result.Failure(ProductErrors.NoUpdates(id));

        if (updates.Count > 0)
        {
            var updateDef = Builders<Product>.Update.Combine(updates);
            await productRepository.UpdateProduct(id, updateDef);
        }

        return Result.Success();
    }

    // TODO delete all of the reviews for the product as well as the image
    // Orders for this product should not be deleted
    public async Task<Result> DeleteAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        await productRepository.DeleteProduct(objectId);
        return Result.Success();
    }
}