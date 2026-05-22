using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RegisterPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";

    [SetUp]
    public async Task NavigateToRegisterPage()
    {
        await Page.GotoAsync($"{BaseUrl}/register");
    }

    private ILocator NameInput => Page.Locator("#name");
    private ILocator SurnameInput => Page.Locator("#surname");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator SignUpButton => Page.Locator("button[type='submit']");
    private ILocator SignInLink => Page.GetByRole(AriaRole.Link, new() { Name = "Sign In" });
    private ILocator SuccessSnackbar => Page.Locator("[class*='notistack-MuiContent-success']");
    private ILocator EmailFieldError => Page.Locator("#email-helper-text");
    private ILocator PasswordFieldError => Page.Locator("#password-helper-text");

    private async Task FillForm(string name, string surname, string email, string password)
    {
        await NameInput.FillAsync(name);
        await SurnameInput.FillAsync(surname);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
    }

    [Test]
    public async Task Register_ValidData_ShowsSuccessSnackbarAndRedirectsToLogin()
    {
        await FillForm("John", "Doe", $"test_{Guid.NewGuid():N}@example.com", "ValidPassword123!");
        await SignUpButton.ClickAsync();

        await Expect(SuccessSnackbar).ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }

    [Test]
    public async Task Register_ExistingEmail_ShowsEmailFieldError()
    {
        await FillForm("John", "Doe", "atest@test.com", "ValidPassword123!");
        await SignUpButton.ClickAsync();

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
        await FillForm("John", "Doe", "notanemail", "ValidPassword123!");
        await SignUpButton.ClickAsync();

        await Expect(EmailFieldError).ToBeVisibleAsync();
    }

    [Test]
    public async Task Register_PasswordTooShort_ShowsPasswordValidationError()
    {
        await FillForm("John", "Doe", "john@example.com", "Ab1");
        await SignUpButton.ClickAsync();

        await Expect(PasswordFieldError).ToHaveTextAsync("Password must be at least 6 characters.");
    }

    [Test]
    public async Task Register_PasswordMissingUppercase_ShowsPasswordValidationError()
    {
        await FillForm("John", "Doe", "john@example.com", "password123");
        await SignUpButton.ClickAsync();

        await Expect(PasswordFieldError).ToHaveTextAsync("Password must contain at least one uppercase letter.");
    }

    [Test]
    public async Task Register_PasswordMissingLowercase_ShowsPasswordValidationError()
    {
        await FillForm("John", "Doe", "john@example.com", "PASSWORD123");
        await SignUpButton.ClickAsync();

        await Expect(PasswordFieldError).ToHaveTextAsync("Password must contain at least one lowercase letter.");
    }
}
