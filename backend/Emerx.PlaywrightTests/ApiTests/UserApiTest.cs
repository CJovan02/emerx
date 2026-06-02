using System.Text.Json;
using EMerx.Auth;
using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

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
}