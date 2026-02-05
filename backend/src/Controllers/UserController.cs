using EMerx.Auth;
using EMerx.DTOs.Email;
using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using EMerx.ResultPattern;
using EMerx.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(IdRequest request)
    {
        return (await userService.GetByIdAsync(request)).ToActionResult();
    }

    [HttpGet("getByFirebaseUid/{firebaseUid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByFirebaseUid(string firebaseUid)
    {
        var result = await userService.GetByFirebaseUidAsync(firebaseUid);

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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        var result = await userService.DeleteAsync(request);

        return result.ToActionResult();
    }
}