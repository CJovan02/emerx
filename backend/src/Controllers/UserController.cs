using EMerx.DTOs.Email;
using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.ResultPattern;
using EMerx.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(IdRequest request)
    {
        return (await userService.GetByIdAsync(request)).ToActionResult();
    }

    [HttpGet("getByFirebaseUid/{firebaseUid}")]
    public async Task<IActionResult> GetByFirebaseUid(string firebaseUid)
    {
        var result = await userService.GetByFirebaseUidAsync(firebaseUid);

        return result.ToActionResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser dto)
    {
        var result = await userService.RegisterAsync(dto);

        return result.ToActionResult();
    }

    [HttpPatch("grantAdminRole/{email}")]
    public async Task<IActionResult> GrantAdminRole([FromRoute] EmailRequest request)
    {
        // Need to check if user trying to access this endpoint is admin

        return (await userService.GrantAdminRoleAsync(request.Email)).ToActionResult();
    }

    [HttpPatch("removeAdminRole/{email}")]
    public async Task<IActionResult> RemoveAdminRole([FromRoute] EmailRequest request)
    {
        // Need to check if user trying to access this endpoint is admin

        return (await userService.RemoveAdminRoleAsync(request.Email)).ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] IdRequest request)
    {
        var result = await userService.DeleteAsync(request);

        return result.ToActionResult();
    }
}