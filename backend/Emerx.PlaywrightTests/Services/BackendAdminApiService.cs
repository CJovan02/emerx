using Emerx.PlaywrightTests.Constants;
using Emerx.PlaywrightTests.Helpers;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.Services;

/// <summary>
/// Service used to call server API while being logged in as Admin.
/// Before using you need to call .Connect()
/// It uses env variables from .env.local file.
/// You need to load env variables from that file in order to use this service.
///
/// I also use it inside RegisterPage.cs to call ApiContext functions by hand.
/// That way it's compatible with older codebase. Note: when using ApiContext function manually,
/// you don't need to call .Connect();
/// </summary>
public class BackendAdminApiService
{
    private readonly IPlaywright _playwright;
    private readonly string _firebaseApiKey;

    private string _adminJwt = null;
    private IAPIRequestContext _request = null;

    private readonly string AdminEmail = AuthHelper.AdminEmail;
    private readonly string AdminPassword = AuthHelper.AdminPassword;

    public BackendAdminApiService(IPlaywright playwright)
    {
        _playwright = playwright;
        _firebaseApiKey = DotNetEnv.Env.GetString(EnvVariables.FirebaseApiKey)
                          ?? throw new Exception("Firebase api key not found.");
    }

    public async Task Connect()
    {
        _adminJwt = await GetFirebaseTokenAsync(AdminEmail, AdminPassword);
        _request = await CreateApiContextAsync(_adminJwt);
    }

    public async Task<string> GetFirebaseTokenAsync(string email, string password)
    {
        var firebaseContext = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = PageUrls.FirebaseTokenUrl,
        });
        var response = await firebaseContext.PostAsync(
            $"v1/accounts:signInWithPassword?key={_firebaseApiKey}",
            new APIRequestContextOptions
            {
                DataObject = new { email, password, returnSecureToken = true }
            });
        var json = await response.JsonAsync();
        await firebaseContext.DisposeAsync();
        return json!.Value.GetProperty("idToken").GetString()!;
    }

    public async Task<IAPIRequestContext> CreateApiContextAsync(string token) =>
        await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"{ServerUrls.Backend}/",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            }
        });

    public async Task<string?> GetCurrentUserIdAsync(string token)
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
}