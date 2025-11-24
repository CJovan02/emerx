using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews.Request;
using EMerx.ResultPattern;
using EMerx.Services.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return (await reviewService.GetAllAsync()).ToActionResult();
    }

    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status404NotFound))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] IdRequest request)
    {
        return (await reviewService.GetByIdAsync(request)).ToActionResult();
    }

    [HttpGet("getProductReviews/{id}")]
    public async Task<IActionResult> GetByProductId([FromRoute] IdRequest request)
    {
        return (await reviewService.GetByProductIdAsync(request)).ToActionResult();
    }
    
    [ProducesResponseType((StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewRequest request)
    {
        return (await reviewService.CreateAsync(request)).ToActionResult();
    }
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        return (await reviewService.DeleteAsync(request)).ToActionResult();
    }
}