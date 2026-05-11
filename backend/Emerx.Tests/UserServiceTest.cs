using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
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
    private const string _userEmail = "johnPeacock@email.com";

    private User CreateUser(ObjectId? id = null, string? email = null, string? firebaseUid = null)
    {
        return new User
        {
            Email = email ?? _userEmail,
            Id = id ?? ObjectId.GenerateNewId(),
            Name = "John",
            Surname = "Peacock",
            FirebaseUid = firebaseUid,
        };
    }

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

        var user = CreateUser(objectId);

        _userRepository
            .Setup(r => r.GetUserById(objectId, null))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(_userEmail));
        });
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsUserNotFoundError()
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

    [Test]
    public async Task RegisterUserAsync_ExistingEmailPassed_ReturnsEmailOccupiedError()
    {
        // Arrange
        var userId = ObjectId.GenerateNewId();
        const string occupiedEmail = "occupied@gmail.com";
        var request = new RegisterUserRequest
        {
            Email = occupiedEmail,
            Password = "ValidPassword123",
            Name = "John",
            Surname = "Peacock",
        };

        var user = CreateUser(userId, occupiedEmail, "SomeValidFirebaseUid");

        _userRepository
            .Setup(r => r.GetUserByEmail(occupiedEmail))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailiure, Is.True);
            Assert.That(result.Error, Is.EqualTo(UserErrors.EmailOccupied(occupiedEmail)));
        });
    }

    [Test]
    public async Task RegisterUserAsync_UserExistsWithoutFirebaseUid_CompleteRegistration()
    {
        // Arrange
        const string existingEmail = "existing@email.com";
        const string validPassword = "ValidPassword123";
        const string validFirebaseUid = "SomeValidFirebaseUid";

        var request = new RegisterUserRequest
        {
            Email = existingEmail,
            Password = validPassword,
            Name = "John",
            Surname = "Peacock",
        };
        var user = CreateUser(null, existingEmail, null);

        _userRepository
            .Setup(r => r.GetUserByEmail(existingEmail))
            .ReturnsAsync(user);

        _authRepository
            .Setup(r => r.RegisterAsync(existingEmail, validPassword))
            .ReturnsAsync(validFirebaseUid);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(existingEmail));
        });

        _userRepository.Verify(
            r => r.CreateUser(It.IsAny<User>()),
            Times.Never);

        _userRepository.Verify(
            r => r.UpdateUser(It.Is<User>(u => u.FirebaseUid == validFirebaseUid)),
            Times.Once);
    }

    [Test]
    public async Task RegisterAsync_NewUser_CompleteRegistration()
    {
        // Arrange
        const string newEmail = "new@email.com";
        const string validPassword = "ValidPassword123";
        const string validFirebaseUid = "SomeValidFirebaseUid";

        var request = new RegisterUserRequest
        {
            Email = newEmail,
            Password = validPassword,
            Name = "John",
            Surname = "Peacock",
        };

        _userRepository
            .Setup(r => r.GetUserByEmail(newEmail))
            .ReturnsAsync((User)null);

        _userRepository
            .Setup(r => r.CreateUser(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _authRepository
            .Setup(r => r.RegisterAsync(newEmail, validPassword))
            .ReturnsAsync(validFirebaseUid);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(newEmail));
        });

        // I wanted here to test if the user object in the moment of calling this function has FirebaseUid field to NULL (which it must)
        // But it looks like that Moq tracks the reference of the mock object and not the snapshot in the moment of calling the function
        // FirebaseUid is changed later to be "SomeValidFirebaseUid" and that's why here we can't check for it to be null.
        _userRepository.Verify(
            r => r.CreateUser(It.Is<User>(u =>
                u.Email == newEmail)),
            Times.Once);

        _authRepository.Verify(
            r => r.RegisterAsync(newEmail, validPassword),
            Times.Once);

        _userRepository.Verify(
            r => r.UpdateUser(It.Is<User>(u => u.FirebaseUid == validFirebaseUid)),
            Times.Once);
    }
}