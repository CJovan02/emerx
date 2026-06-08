using System.Text.Json;
using EMerx.Auth;
using EMerx.DTOs.Address;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MongoDB.Bson;

namespace Emerx.PlaywrightTests.ApiTests;

[TestFixture]
public class UserApiTest : PlaywrightTest
{
    private IAPIRequestContext _request;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    [SetUp]
    public async Task SetUp()
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };

        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ServerUrls.BackendTest,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true,
        });
    }

    [Test]
    public async Task GetSelf_ReturnsAuthorizedUser()
    {
        // Arrange
        // Act
        await using var response = await _request.GetAsync(UserUrls.Base);

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var user = await response.JsonAsync<UserResponse>(_jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Email, Is.EqualTo(TestUsers.Admin.Email));
        });
    }

    [Test]
    public async Task Register_ValidRequest_ReturnsNewUser()
    {
        // Arrange
        var newEmail = "playwright-api-test@test.com";
        var data = UserApiHelpers.CreateRegisterUserRequest(newEmail);

        UserResponse createdUser = null;

        // Act
        try
        {
            await using var response = await _request.PostAsync(UserUrls.Register, new APIRequestContextOptions
            {
                DataObject = data
            });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            createdUser = await response.JsonAsync<UserResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(createdUser, Is.Not.Null);
                Assert.That(createdUser.Email, Is.EqualTo(newEmail));
            });
        }
        finally
        {
            if (createdUser is not null)
                await UserApiHelpers.DeleteUser(_request, createdUser.Id);
        }
    }

    [Test]
    public async Task Register_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var newEmail = "invalid-email-format";
        var data = UserApiHelpers.CreateRegisterUserRequest(newEmail);

        // Act
        await using var response = await _request.PostAsync(UserUrls.Register, new APIRequestContextOptions
        {
            DataObject = data
        });

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task GrantAdminRole_AnotherUserEmail_ReturnsOk()
    {
        // Arrange
        var otherUserEmail = TestUsers.User.Email;

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync($"{UserUrls.GrantAdminRole}/{otherUserEmail}");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            // There is nothing more to test, this function modifies the JWT claims.
            // We have no valid way to obtain the Claims of this user, so I only assert the response status code
        }
        finally
        {
            // Cleanup, remove the admin role
            await _request.PatchAsync($"{UserUrls.RemoveAdminRole}/{otherUserEmail}");
        }
    }

    [Test]
    public async Task GrantAdminRole_CurrentUserEmail_ReturnsConflict()
    {
        // This user is injected into auth pipeline on server when you run it in Api-Testing profile
        var currentUserEmail = TestUsers.User.Email;

        await using var response = await _request.PostAsync($"{UserUrls.GrantAdminRole}/{currentUserEmail}");

        Assert.That(response.Status, Is.EqualTo(405));
    }

    [Test]
    public async Task RemoveAdminRole_AnotherUserEmail_ReturnsOk()
    {
        // Arrange
        var otherUserEmail = TestUsers.User.Email;
        // Act
        await using var response =
            await _request.PatchAsync($"{UserUrls.RemoveAdminRole}/{otherUserEmail}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        // There is nothing more to test, this function modifies the JWT claims.
        // We have no valid way to obtain the Claims of this user, so I only assert the response status code
    }

    [Test]
    public async Task RemoveAdminRole_CurrentUserEmail_ReturnsConflict()
    {
        // This user is injected into auth pipeline on server when you run it in Api-Testing profile
        var currentUserEmail = TestUsers.User.Email;

        await using var response = await _request.PostAsync($"{UserUrls.RemoveAdminRole}/{currentUserEmail}");

        Assert.That(response.Status, Is.EqualTo(405));
    }

    // /User/Delete endpoint is not good idea to test. It deletes the user that is sending the request
    // That means it will delete our user that we are using for api testing

    [Test]
    public async Task UpdateUser_ValidRequest_ReturnsUpdatedUser()
    {
        // Arrange
        var createdUser = await UserApiHelpers.RegisterUser(_request);
        var updateRequest = UserApiHelpers.CreateUpdateUserRequest();

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync($"{UserUrls.Base}/{createdUser.Id}", new APIRequestContextOptions
                {
                    DataObject = updateRequest
                });

            Assert.That(response.Status, Is.EqualTo(200));

            var updatedUser = await response.JsonAsync<UserResponse>(_jsonOptions);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser, Is.Not.Null);
                Assert.That(updatedUser.Name, Is.EqualTo(updateRequest.Name));
                Assert.That(updatedUser.Surname, Is.EqualTo(updateRequest.Surname));
                Assert.That(updatedUser.Address.City, Is.EqualTo(updateRequest.Address.City));
            });
        }
        finally
        {
            await UserApiHelpers.DeleteUser(_request, createdUser.Id);
        }
    }

    [Test]
    public async Task UpdateUser_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var createdUser = await UserApiHelpers.RegisterUser(_request);
        var tooLongName = "this-name-is-very-long-and-it-wont-pass-server-validation";
        var invalidRequest = UserApiHelpers.CreateUpdateUserRequest(tooLongName);

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync($"{UserUrls.Base}/{createdUser.Id}", new APIRequestContextOptions
                {
                    DataObject = invalidRequest
                });

            // Assert
            Assert.That(response.Status, Is.EqualTo(400));
        }
        finally
        {
            await UserApiHelpers.DeleteUser(_request, createdUser.Id);
        }
    }

    [Test]
    public async Task DeleteUser_ValidId_ReturnsOk()
    {
        // Arrange
        var createdUser = await UserApiHelpers.RegisterUser(_request);

        // Act
        await using var response = await _request.DeleteAsync($"{UserUrls.Base}/{createdUser.Id}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        // There is no getUserById endpoint, so we just check status-code == 200
    }

    [Test]
    public async Task DeleteUser_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response = await _request.DeleteAsync($"{UserUrls.Base}/{nonExistingId}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }
}