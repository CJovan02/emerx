using EMerx_backend.Features.Products.Dtos;
using EMerx_backend.Features.Products.Errors;
using EMerx_backend.Features.Products.Repositories;
using FluentResults;
using Mapster;
using MongoDB.Bson;

namespace EMerx_backend.Features.Products.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<Result<IEnumerable<ProductDto>>> GetProducts()
    {
        return Result.Ok((await _productRepository.GetProducts())
            .Adapt<IEnumerable<ProductDto>>());
    }

    public async Task<Result<ProductDto>> GetProduct(ObjectId id)
    {
        var product = await _productRepository.GetProductById(id);
        if(product is null)
            return Result.Fail<ProductDto>(new ProductNotFoundError(id));
        return Result.Ok(product.Adapt<ProductDto>());
    }

    public async Task<Result<ProductDto>> CreateProduct(CreateProductDto productDto)
    {
        var product = productDto.Adapt<Product>();
        await _productRepository.CreateProduct(product);
        return Result.Ok(product.Adapt<ProductDto>());
    }

    public async Task<Result<ProductDto>> UpdateProduct(ProductDto productDto)
    {
        var product = await _productRepository.GetProductById(productDto.Id);
        if (product is null)
            return Result.Fail<ProductDto>(new ProductNotFoundError(productDto.Id));
        
        product.Category = productDto.Category;
        product.Image = productDto.Image;
        product.Price = productDto.Price;
        product.Name = productDto.Name;
        await _productRepository.UpdateProduct(product);
        
        return Result.Ok(product.Adapt<ProductDto>());
    }

    public async Task<Result> DeleteProduct(ObjectId id)
    {
        var product = await _productRepository.GetProductById(id);
        if (product is null)
            return Result.Fail(new ProductNotFoundError(id));
        await _productRepository.DeleteProduct(id);
        return Result.Ok();
    }
    
}