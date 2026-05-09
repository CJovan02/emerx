using EMerx.DTOs.Id;
using EMerx.Entities;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern.Errors;
using EMerx.Services.Users;
using MongoDB.Bson;
using Moq;

namespace Emerx.Tests;

public class UserServiceTest
{
    private UserService _userService;
    private Mock<IUserRepository> _userRepository;
    private Mock<IAuthRepository> _authRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<IUserRepository>();
        _authRepository = new Mock<IAuthRepository>();

        _userService = new UserService(_userRepository.Object, _authRepository.Object);
    }

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsUser()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();
        var request = new IdRequest
        {
            Id = objectId.ToString()
        };
        var userEmail = "johnPeacock@email.com";

        var user = new User
        {
            Id = objectId,
            Email = userEmail,
            Name = "John",
            Surname = "Peacock",
        };

        _userRepository
            .Setup(r => r.GetUserById(objectId, null))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(userEmail));
        });
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsFailure()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();
        var request = new IdRequest
        {
            Id = objectId.ToString()
        };

        _userRepository
            .Setup(r => r.GetUserById(objectId, null))
            .ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);
            Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound(objectId)));
        });
    }
}