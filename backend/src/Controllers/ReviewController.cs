using EMerx.Auth;
using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using EMerx.ResultPattern;
using EMerx.Services.Reviews;
using EMerx.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PageParams pageParams)
    {
        return (await reviewService.GetAllAsync(pageParams.Page, pageParams.PageSize)).ToActionResult();
    }

    [Authorize]
    [ProducesResponseType(typeof(ReviewResponse), (StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status404NotFound))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] IdRequest request)
    {
        return (await reviewService.GetByIdAsync(request)).ToActionResult();
    }

    [Authorize]
    [HttpGet("getProductReviews/{id}")]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> GetByProductId([FromRoute] IdRequest request)
    {
        return (await reviewService.GetByProductIdAsync(request)).ToActionResult();
    }

    [Authorize]
    [ProducesResponseType((StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewRequest request)
    {
        var uid = JwtUtils.GetUidFromHttpContext(HttpContext);
        if (uid is null)
            return Unauthorized();
        return (await reviewService.CreateAsync(uid, request)).ToActionResult();
    }

    [Authorize]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch([FromRoute] IdRequest idRequest, [FromBody] PatchReviewRequest request)
    {
        var uid = JwtUtils.GetUidFromHttpContext(HttpContext);
        if (uid is null)
            return Unauthorized();

        return (await reviewService.PatchAsync(idRequest, request, uid)).ToActionResult();
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        return (await reviewService.DeleteAsync(request)).ToActionResult();
    }
}