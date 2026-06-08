using EMerx.Common.Filters;
using EMerx.DTOs.Products.Response;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Common;
using Emerx.PlaywrightTests.Constants;
using Emerx.PlaywrightTests.Helpers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.Services;

/// <summary>
/// Service used to call server API.
/// Before using you need to call Connect(userToken);
/// UserToken is used to authenticate the person which userToken belongs to.
/// You can use AuthHelper.GetFirebaseTokenAsync() function to obtain a token.
/// </summary>
public class BackendApiService : IAsyncDisposable
{
    private readonly IPlaywright _playwright;

    private string? _idToken;
    private IAPIRequestContext? _request;

    private readonly string AdminEmail = AuthHelper.AdminEmail;
    private readonly string AdminPassword = AuthHelper.AdminPassword;

    public BackendApiService(IPlaywright playwright)
    {
        _playwright = playwright;
    }

    /// <summary>
    /// Attaches userToken to every request you make with this instance of api.
    /// In other words, it authorizes user sending request with this instance with a provided userToken.
    /// </summary>
    public async Task ConnectAsync(string userToken)
    {
        _idToken = userToken;
        _request = await CreateApiContextAsync(_idToken);
    }

    public async Task<UserResponse> GetUserByEmailAsync(string email)
    {
        await using var response = await _request.GetAsync($"{UserUrls.Base}/{email}");
        if (!response.Ok)
            return null;

        var user = await response.JsonAsync<UserResponse>(JsonSerializers.CaseInsensitive);

        return user;
    }

    public async Task<UserResponse?> GetUserSelfAsync()
    {
        await using var response = await _request.GetAsync(UserUrls.Base);
        if (!response.Ok)
            return null;

        var user = await response.JsonAsync<UserResponse>(JsonSerializers.CaseInsensitive);

        return user;
    }

    public async Task<UserResponse> RegisterUserAsync(string email, string password, string name, string surname)
    {
        var registerRequest = new RegisterUserRequest
        {
            Name = name,
            Surname = surname,
            Email = email,
            Password = password,
        };

        await using var response = await _request.PostAsync(UserUrls.Register, new APIRequestContextOptions
        {
            DataObject = registerRequest,
        });

        if (!response.Ok)
            throw new Exception("Can't register user");

        return (await response.JsonAsync<UserResponse>(JsonSerializers.CaseInsensitive));
    }

    public async Task DeleteUserSelfAsync()
    {
        await using var response = await _request.DeleteAsync(UserUrls.Base);
        if (!response.Ok)
            throw new Exception("Can't delete user");
    }

    // I already use helper functions in API tests. I just reused them here :)
    public async Task DeleteUserAsync(string userId)
    {
        await UserApiHelpers.DeleteUser(_request, userId);
    }

    public async Task<ProductResponse> CreateProduct(string productName = "Test Name")
    {
        return await ProductApiHelpers.PostProduct(_request, productName);
    }

    public async Task DeleteProduct(string productId)
    {
        await ProductApiHelpers.DeleteProduct(_request, productId);
    }

    public async Task<ProductResponse?> GetProductByName(string name)
    {
        const int page = 1;
        const int pageSize = 1;
        await using var response =
            await _request.GetAsync($"{ProductUrls.Base}?page={page}&pageSize={pageSize}&search={name}");

        if (!response.Ok)
            return null;

        return (await response.JsonAsync<PageOfResponse<ProductResponse>>(JsonSerializers.CaseInsensitive)).Items
            .FirstOrDefault();
    }

    private async Task<IAPIRequestContext> CreateApiContextAsync(string token) =>
        await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"{ServerUrls.Backend}/",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" },
                { "Accept", "application/json" }
            }
        });

    /// <summary>
    /// Older api. It creates apiContext in this function and disposes it afterward.
    /// </summary>
    public async Task<string?> GetCurrentUserIdAsyncTransient(string token)
    {
        var context = await CreateApiContextAsync(token);
        var response = await context.GetAsync("user");
        if (!response.Ok)
        {
            await context.DisposeAsync();
            return null;
        }

        var json = await response.JsonAsync();
        await context.DisposeAsync();
        return json!.Value.GetProperty("id").GetString();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_request is not null)
            await _request.DisposeAsync();
    }
}