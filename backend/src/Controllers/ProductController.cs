using EMerx.DTOs.Id;
using EMerx.DTOs.Products.Request;
using EMerx.DTOs.Products.Response;
using EMerx.ResultPattern;
using EMerx.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    // Add pagination
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return (await productService.GetAllAsync()).ToActionResult();
    }

    [ProducesResponseType(typeof(ProductResponse), (StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status404NotFound))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] IdRequest request)
    {
        return (await productService.GetByIdAsync(request)).ToActionResult();
    }

    [ProducesResponseType(typeof(ProductResponse), (StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        return (await productService.CreateAsync(request)).ToActionResult();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        return (await productService.DeleteAsync(request)).ToActionResult();
    }
}