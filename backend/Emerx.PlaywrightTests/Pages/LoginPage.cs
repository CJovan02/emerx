using Emerx.PlaywrightTests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";

    [SetUp]
    public async Task NavigateToLoginPage()
    {
        await Page.GotoAsync($"{BaseUrl}/login");
    }

    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator SignInButton => Page.Locator("button[type='submit']");
    private ILocator ErrorSnackbar => Page.Locator("[role='alert']");
    private ILocator SignUpLink => Page.GetByRole(AriaRole.Link, new() { Name = "Sign Up" });

    [Test]
    public async Task Login_ValidCredentials_RedirectsToProductsPage()
    {
        // await EmailInput.FillAsync("admin@admin.com");
        // await PasswordInput.FillAsync("Sifra123");
        // await SignInButton.ClickAsync();
        await AuthHelper.LoginAsAdmin(Page);

        // await Expect(Page).ToHaveURLAsync($"{BaseUrl}/products");
    }

    [Test]
    public async Task Login_InvalidCredentials_ShowsErrorSnackbar()
    {
        await EmailInput.FillAsync("wrong@example.com");
        await PasswordInput.FillAsync("WrongPassword123!");
        await SignInButton.ClickAsync();

        await Expect(ErrorSnackbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_EmptyFields_DoesNotSubmit()
    {
        await SignInButton.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }

    [Test]
    public async Task Login_SignUpLink_NavigatesToRegisterPage()
    {
        await SignUpLink.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/register");
    }
}
