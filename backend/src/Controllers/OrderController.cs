using EMerx.Auth;
using EMerx.Common.Filters;
using EMerx.DTOs.Id;
using EMerx.DTOs.Orders.Request;
using EMerx.DTOs.Orders.Response;
using EMerx.ResultPattern;
using EMerx.Services.Orders;
using EMerx.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PageParams pageParams)
    {
        return (await orderService.GetAllAsync(pageParams.Page, pageParams.PageSize)).ToActionResult();
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] IdRequest request)
    {
        return (await orderService.GetByIdAsync(request)).ToActionResult();
    }

    [Authorize]
    [ProducesResponseType(typeof(OrderReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Review([FromBody] OrderReviewRequest request)
    {
        return (await orderService.GetOrderReview(request)).ToActionResult();
    }


    [Authorize]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderRequest request)
    {
        var uid = JwtUtils.GetUidFromHttpContext(HttpContext);
        if (uid is null)
            return Unauthorized();

        return (await orderService.CreateAsync(uid, request)).ToActionResult();
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        return (await orderService.DeleteAsync(request)).ToActionResult();
    }
}