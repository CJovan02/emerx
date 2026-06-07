using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.Helpers;

public static class AuthHelper
{
    public const string AdminEmail = "test-admin@test.com";
    public const string AdminPassword = "Sifra123";

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
        var apiKey = DotNetEnv.Env.GetString(EnvVariables.FirebaseApiKey)
                     ?? throw new Exception("Firebase api key not found.");

        var firebaseContext = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = PageUrls.FirebaseTokenUrl,
        });
        var response = await firebaseContext.PostAsync(
            $"v1/accounts:signInWithPassword?key={apiKey}",
            new APIRequestContextOptions
            {
                DataObject = new { email, password, returnSecureToken = true }
            });
        var json = await response.JsonAsync();
        await firebaseContext.DisposeAsync();
        return json!.Value.GetProperty("idToken").GetString()!;
    }
}