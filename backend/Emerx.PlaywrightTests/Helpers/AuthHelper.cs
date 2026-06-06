using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Helpers;

public static class AuthHelper
{
    private static readonly string AdminEmail = "admin@admin.com";
    private static readonly string AdminPassword = "Sifra123";

    public static async Task LoginAsAdmin(IPage page)
    {
        await page.GotoAsync(PageUrls.LoginPage);

        await page.GetByLabel("Email")
            .FillAsync(AdminEmail);
        await page.GetByLabel("Password")
            .FillAsync("Sifra123");
        await page.GetByTestId("login-button").ClickAsync();

        await page.WaitForURLAsync(PageUrls.ProductsPage);
    }

    public static async Task<string> GetFirebaseTokenAsync(IPlaywright playwright, string email, string password)
    {
        var firebaseContext = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = PageUrls.FirebaseTokenUrl,
        });
        var response = await firebaseContext.PostAsync(
            $"v1/accounts:signInWithPassword?key={FirebaseApiKey}",
            new APIRequestContextOptions
            {
                DataObject = new { email, password, returnSecureToken = true }
            });
        var json = await response.JsonAsync();
        await firebaseContext.DisposeAsync();
        return json!.Value.GetProperty("idToken").GetString()!;
    }

    public static async Task<IAPIRequestContext> CreateApiContextAsync(string token) =>
        await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"{BackendUrl}/",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            }
        });

    public static async Task<string?> GetCurrentUserIdAsync(string token)
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