using Emerx.PlaywrightTests.Helpers;
using Emerx.PlaywrightTests.Services;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RegisterPage : PageTest
{
    private BackendApiService _api;

    private const string BaseUrl = "http://localhost:5173";
    private const string TestPassword = "ValidPassword123!";

    private string _email = string.Empty;
    private ILocator NameInput => Page.Locator("#name");
    private ILocator SurnameInput => Page.Locator("#surname");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator SignUpButton => Page.Locator("button[type='submit']");
    private ILocator SignInLink => Page.GetByRole(AriaRole.Link, new() { Name = "Sign In" });
    private ILocator SuccessSnackbar => Page.Locator("[class*='notistack-MuiContent-success']");
    private ILocator EmailFieldError => Page.Locator("#email-helper-text");
    private ILocator PasswordFieldError => Page.Locator("#password-helper-text");

    [SetUp]
    public async Task NavigateToRegisterPage()
    {
        _api = new BackendApiService(Playwright);
        await _api.ConnectAsync(GlobalSetup.AdminToken);

        await Page.GotoAsync($"{BaseUrl}/register");
    }

    [Test]
    public async Task Register_ValidData_ShowsSuccessSnackbarAndRedirectsToLogin()
    {
        _email = $"test_{Guid.NewGuid():N}@example.com";
        await Register(_email);
        await Expect(SuccessSnackbar).ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }

    [Test]
    public async Task Register_ExistingEmail_ShowsEmailFieldError()
    {
        await Register(AuthHelper.AdminEmail);
        await Expect(EmailFieldError).ToBeVisibleAsync();
        await Expect(EmailFieldError).ToHaveTextAsync("That Email is already in use.");
    }

    [Test]
    public async Task Register_EmptyFields_DoesNotSubmit()
    {
        await SignUpButton.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/register");
    }

    [Test]
    public async Task Register_SignInLink_NavigatesToLoginPage()
    {
        await SignInLink.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }

    [Test]
    public async Task Register_InvalidEmailFormat_ShowsEmailValidationError()
    {
        await Register("notanemail");
        await Expect(EmailFieldError).ToBeVisibleAsync();
    }

    [Test]
    public async Task Register_PasswordTooShort_ShowsPasswordValidationError()
    {
        await Register("john@example.com", "Aa");
        await SignUpButton.ClickAsync();
        await Expect(PasswordFieldError).ToHaveTextAsync("Password must be at least 6 characters.");
    }

    [Test]
    public async Task Register_PasswordMissingUppercase_ShowsPasswordValidationError()
    {
        await Register("john@example.com", "lowercase1!");
        await Expect(PasswordFieldError).ToHaveTextAsync("Password must contain at least one uppercase letter.");
    }

    [Test]
    public async Task Register_PasswordMissingLowercase_ShowsPasswordValidationError()
    {
        await Register("john@example.com", "UPPERCASE1!");
        await Expect(PasswordFieldError).ToHaveTextAsync("Password must contain at least one lowercase letter.");
    }

    [TearDown]
    public async Task CleanUpCreatedUser()
    {
        if (string.IsNullOrEmpty(_email)) return;

        var user = await _api.GetUserByEmailAsync(_email);

        if (user is not null)
            await _api.DeleteUserAsync(user.Id);

        _email = string.Empty;
        await _api.DisposeAsync();
    }

    private async Task Register(string email, string? password = null)
    {
        await FillForm("John", "Doe", email, password ?? TestPassword);
        await SignUpButton.ClickAsync();
    }

    private async Task FillForm(string name, string surname, string email, string password)
    {
        await NameInput.FillAsync(name);
        await SurnameInput.FillAsync(surname);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
    }
}