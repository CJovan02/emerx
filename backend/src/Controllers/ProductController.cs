using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.ResultPattern;
using EMerx.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    // Add pagination
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        return (await productService.GetAllProductsAsync()).ToActionResult();
    }

    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status404NotFound))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        return (await productService.GetProductByIdAsync(new IdRequest(id))) .ToActionResult();
    }

    [ProducesResponseType((StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
    {
        return (await  productService.CreateProductAsync(request)).ToActionResult();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        return (await productService.DeleteProductAsync(new IdRequest(id))).ToActionResult();
    }
}