using EMerx.DTOs.Users;
using EMerx.ResultPattern;
using EMerx.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace EMerx.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(new { error = "Invalid Id" });

        var result = await userService.GetUserById(objectId);

        return result.ToActionResult();
    }

    [HttpGet("getByFirebaseUid/{firebaseUid}")]
    public async Task<IActionResult> GetUserByFirebaseUid(string firebaseUid)
    {
        var result = await userService.GetUserByFirebaseUid(firebaseUid);

        return result.ToActionResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await userService.RegisterAsync(dto);

        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(new { error = "Invalid Id" });

        var result = await userService.DeleteUserAsync(objectId);

        return result.ToActionResult();
    }
}