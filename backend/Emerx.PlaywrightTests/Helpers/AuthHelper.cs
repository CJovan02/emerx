using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.Helpers;

public static class AuthHelper
{
    public readonly static string AdminEmail = "admin@admin.com";
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
}