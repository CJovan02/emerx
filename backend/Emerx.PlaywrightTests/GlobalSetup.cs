using Emerx.PlaywrightTests.Helpers;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests;

[SetUpFixture]
public class GlobalSetup
{
    // We only need to get the token once and reuse it every other API request.
    // If not for this, EVERY TEST would call the Google server for the admin token.
    public static string AdminToken { get; private set; }

    [OneTimeSetUp]
    public async Task Setup()
    {
        var root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../")
        );

        var envPath = Path.Combine(root, "src", ".env.local");

        DotNetEnv.Env.Load(envPath);

        var playwright = await Playwright.CreateAsync();
        AdminToken =
            await AuthHelper.GetFirebaseTokenAsync(playwright, AuthHelper.AdminEmail, AuthHelper.AdminPassword);

        playwright.Dispose();
    }
}