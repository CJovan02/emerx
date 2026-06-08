using System.Text.Json;
using EMerx.DTOs.Address;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.ApiTests.Helpers;

public static class UserApiHelpers
{
    public static RegisterUserRequest CreateRegisterUserRequest(string email = "playwright-api-test@email.com")
    {
        return new RegisterUserRequest
        {
            Email = email,
            Name = "Playwright",
            Password = "Playwright123",
            Surname = "Playwright",
        };
    }

    public static UpdateUserRequest CreateUpdateUserRequest(string name = "new-name")
    {
        return new UpdateUserRequest
        {
            Name = name,
            Surname = "new-surname",
            Address = new AddressDto
            {
                City = "new-city",
                HouseNumber = "999",
                Street = "new-street"
            }
        };
    }

    public static async Task<UserResponse> RegisterUser(IAPIRequestContext request)
    {
        var data = CreateRegisterUserRequest();

        await using var response = await request.PostAsync(UserUrls.Register, new APIRequestContextOptions
        {
            DataObject = data
        });

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        return await response.JsonAsync<UserResponse>(options);
    }

    public static async Task DeleteUser(IAPIRequestContext request, string userId)
    {
        await request.DeleteAsync($"{UserUrls.Base}/{userId}");
    }
}