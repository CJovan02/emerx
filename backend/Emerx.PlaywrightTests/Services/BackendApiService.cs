using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Common;
using Emerx.PlaywrightTests.Constants;
using Emerx.PlaywrightTests.Helpers;
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

    private string? _adminJwt;
    private IAPIRequestContext? _request;

    private readonly string AdminEmail = AuthHelper.AdminEmail;
    private readonly string AdminPassword = AuthHelper.AdminPassword;

    public BackendApiService(IPlaywright playwright)
    {
        _playwright = playwright;
    }

    public async Task ConnectAsync(string userToken)
    {
        _adminJwt = userToken;
        _request = await CreateApiContextAsync(_adminJwt);
    }

    public async Task<string?> GetCurrentUserIdAsync(string token)
    {
        var response = await _request.GetAsync(UserUrls.Base);
        if (!response.Ok)
            return null;

        var user = await response.JsonAsync<UserResponse>(JsonSerializers.CaseInsensitive);

        return user?.Id;
    }

    // I already use helper functions in API tests. I just reused them here :)
    public async Task DeleteUserAsync(string userId)
    {
        await UserApiHelpers.DeleteUser(_request, userId);
    }

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

    private async Task<IAPIRequestContext> CreateApiContextAsync(string token) =>
        await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"{ServerUrls.Backend}/",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            }
        });

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_request is not null)
            await _request.DisposeAsync();
    }
}