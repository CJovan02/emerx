using System.Security.Claims;
using EMerx.Controllers;
using EMerx.DTOs.Address;
using EMerx.DTOs.Email;
using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests.Controllers;

public class UserControllerTest
{
    private UserController _userController;
    private Mock<IUserService> _userService;

    private const string _authorizedUserEmail = "authorized@email.com";
    private const string _firebaseUid = "firebase-uid";
    private DefaultHttpContext _emptyHttpContext;
    private DefaultHttpContext _httpContextWithAdminUser;

    [SetUp]
    public void Setup()
    {
        _userService = new Mock<IUserService>();
        _userController = new UserController(_userService.Object);

        var claims = new List<Claim>
        {
            new("user_id", _firebaseUid),
            new(ClaimTypes.Email, _authorizedUserEmail),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");

        _httpContextWithAdminUser = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        _emptyHttpContext = new DefaultHttpContext();
    }

    [Test]
    public async Task GetSelf_UserAuthorized_ReturnsUser()
    {
        _userController.ControllerContext.HttpContext = _httpContextWithAdminUser;
        var userResponse = CreateUserResponse();

        _userService.Setup(r => r.GetByFirebaseUidAsync(_firebaseUid))
            .ReturnsAsync(userResponse);

        var result = await _userController.GetSelf();

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _userService.Verify(
            s => s.GetByFirebaseUidAsync(_firebaseUid),
            Times.Once);
    }

    [Test]
    public async Task GetSelf_UserUnauthorized_ReturnsUnauthorized()
    {
        _userController.ControllerContext.HttpContext = _emptyHttpContext;

        var result = await _userController.GetSelf();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        _userService.Verify(
            s => s.GetByFirebaseUidAsync(_firebaseUid),
            Times.Never);
    }

    [Test]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        var request = new RegisterUserRequest
        {
            Email = "test@email.com",
            Surname = "Test",
            Name = "Test",
            Password = "SomeValidStrongPassword123"
        };

        var userResponse = CreateUserResponse();

        _userService.Setup(r => r.RegisterAsync(request)).ReturnsAsync(userResponse);

        var result = await _userController.Register(request);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _userService.Verify(
            s => s.RegisterAsync(request),
            Times.Once);
    }

    [Test]
    public async Task Register_ExistingEmail_ReturnsOccupiedError()
    {
        const string existingEmail = "existing@email.com";
        var request = new RegisterUserRequest
        {
            Email = existingEmail,
            Surname = "Test",
            Name = "Test",
            Password = "SomeValidStrongPassword123"
        };

        _userService
            .Setup(r => r.RegisterAsync(request))
            .ReturnsAsync(
                Result<UserResponse>.Failure(UserErrors.EmailOccupied(existingEmail))
            );

        var result = await _userController.Register(request);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());

        _userService.Verify(
            s => s.RegisterAsync(request),
            Times.Once);
    }

    [Test]
    public async Task GrantAdminRole_ValidRequest_ReturnsOk()
    {
        _userController.ControllerContext.HttpContext = _httpContextWithAdminUser;
        var request = new EmailRequest
        {
            Email = "anotheruser@email.com"
        };

        _userService
            .Setup(s => s.GrantAdminRoleAsync(request.Email))
            .ReturnsAsync(Result.Success());


        var result = await _userController.GrantAdminRole(request);


        Assert.That(result, Is.InstanceOf<OkResult>());

        _userService.Verify(
            r => r.GrantAdminRoleAsync(request.Email),
            Times.Once);
    }

    [Test]
    public async Task GrantAdminRole_UserChangingHisRole_ReturnsConflict()
    {
        // inside httpContextWithUser is this _authorizedUserEmail, meaning user is trying to change his own role
        _userController.ControllerContext.HttpContext = _httpContextWithAdminUser;
        var request = new EmailRequest
        {
            Email = _authorizedUserEmail,
        };

        var result = await _userController.GrantAdminRole(request);


        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());

        _userService.Verify(
            r => r.GrantAdminRoleAsync(request.Email),
            Times.Never);
    }


    [Test]
    public async Task RemoveAdminRole_ValidRequest_ReturnsOk()
    {
        _userController.ControllerContext.HttpContext = _httpContextWithAdminUser;
        var request = new EmailRequest
        {
            Email = "anotheruser@email.com"
        };

        _userService
            .Setup(s => s.RemoveAdminRoleAsync(request.Email))
            .ReturnsAsync(Result.Success());


        var result = await _userController.RemoveAdminRole(request);


        Assert.That(result, Is.InstanceOf<OkResult>());

        _userService.Verify(
            r => r.RemoveAdminRoleAsync(request.Email),
            Times.Once);
    }

    [Test]
    public async Task RemoveAdminRole_UserChangingHisRole_ReturnsConflict()
    {
        // inside httpContextWithUser is this _authorizedUserEmail, meaning user is trying to change his own role
        _userController.ControllerContext.HttpContext = _httpContextWithAdminUser;
        var request = new EmailRequest
        {
            Email = _authorizedUserEmail,
        };

        var result = await _userController.RemoveAdminRole(request);


        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());

        _userService.Verify(
            r => r.RemoveAdminRoleAsync(request.Email),
            Times.Never);
    }

    [Test]
    public async Task Delete_UserAuthenticated_ReturnsOk()
    {
        // Arrange

        _userController.ControllerContext.HttpContext =  _httpContextWithAdminUser;

        _userService
            .Setup(s => s.DeleteByFirebaseIdAsync(_firebaseUid))
            .ReturnsAsync(Result.Success());

        // Act

        var result = await _userController.Delete();

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _userService.Verify(
            s => s.DeleteByFirebaseIdAsync(_firebaseUid),
            Times.Once);
    }

    [Test]
    public async Task Delete_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = _emptyHttpContext
        };

        // Act

        var result = await _userController.Delete();

        // Assert

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        _userService.Verify(
            s => s.DeleteByFirebaseIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange

        var userId = ObjectId.GenerateNewId().ToString();

        var request = new IdRequest
        {
            Id = userId
        };

        var dto = new UpdateUserRequest
        {
            Name = "John",
            Surname = "Peacock",
            Address = new AddressDto
            {
                City = "Belgrade",
                Street = "Main Street",
                HouseNumber = "10"
            }
        };

        var response = CreateUserResponse();

        _userService
            .Setup(s => s.UpdateAsync(
                It.Is<IdRequest>(r => r.Id == userId),
                dto))
            .ReturnsAsync(Result<UserResponse>.Success(response));

        // Act

        var result = await _userController.Update(userId, dto);

        // Assert

        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        _userService.Verify(
            s => s.UpdateAsync(
                It.Is<IdRequest>(r => r.Id == userId),
                dto),
            Times.Once);
    }

    [Test]
    public async Task Update_UserDoesNotExist_ReturnsNotFound()
    {
        // Arrange

        var userId = ObjectId.GenerateNewId().ToString();

        var dto = new UpdateUserRequest
        {
            Name = "John",
            Surname = "Peacock",
            Address = new AddressDto
            {
                City = "Belgrade",
                Street = "Main Street",
                HouseNumber = "10"
            }
        };

        _userService
            .Setup(s => s.UpdateAsync(
                It.Is<IdRequest>(r => r.Id == userId),
                dto))
            .ReturnsAsync(Result<UserResponse>.Failure(
                UserErrors.NotFound(userId)));

        // Act

        var result = await _userController.Update(userId, dto);

        // Assert

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        _userService.Verify(
            s => s.UpdateAsync(
                It.Is<IdRequest>(r => r.Id == userId),
                dto),
            Times.Once);
    }



    [Test]
    public async Task DeleteUser_ValidRequest_ReturnsOk()
    {
        // Arrange

        var userId = ObjectId.GenerateNewId().ToString();
        var request = new IdRequest { Id = userId };

        _userService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Success());

        // Act

        var result = await _userController.DeleteUser(request);

        // Assert

        Assert.That(result, Is.InstanceOf<OkResult>());

        _userService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    [Test]
    public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound()
    {
        // Arrange

        var userId = ObjectId.GenerateNewId();
        var request = new IdRequest { Id = userId.ToString() };

        _userService
            .Setup(s => s.DeleteAsync(request))
            .ReturnsAsync(Result.Failure(UserErrors.NotFound(userId)));

        // Act

        var result = await _userController.DeleteUser(request);

        // Assert

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

        _userService.Verify(
            s => s.DeleteAsync(request),
            Times.Once);
    }

    private UserResponse CreateUserResponse()
    {
        return new UserResponse
        {
            Id = "UserId",
            Name = "Name",
            Surname = "Surname",
            Address = new AddressDto
            {
                City = "City",
                HouseNumber = "HouseNumber",
                Street = "Street",
            },
            Email = "Email",
        };
    }
}