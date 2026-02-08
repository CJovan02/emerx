using EMerx.Auth;
using EMerx.DTOs.Email;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using EMerx.ResultPattern;
using EMerx.Services.Users;
using EMerx.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSelf()
    {
        var uid = JwtUtils.GetUidFromHttpContext(HttpContext);
        if (uid is null)
            return Unauthorized();
        var result = await userService.GetByFirebaseUidAsync(uid);

        return result.ToActionResult();
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUser dto)
    {
        var result = await userService.RegisterAsync(dto);

        return result.ToActionResult();
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [HttpPatch("grantAdminRole/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GrantAdminRole([FromRoute] EmailRequest request)
    {
        return (await userService.GrantAdminRoleAsync(request.Email)).ToActionResult();
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [HttpPatch("removeAdminRole/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAdminRole([FromRoute] EmailRequest request)
    {
        return (await userService.RemoveAdminRoleAsync(request.Email)).ToActionResult();
    }

    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete()
    {
        var uid = JwtUtils.GetUidFromHttpContext(HttpContext);
        if (uid is null)
            return Unauthorized();
        var result = await userService.DeleteByFirebaseIdAsync(uid);

        return result.ToActionResult();
    }
}