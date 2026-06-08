using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RegisterPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";
    private const string BackendUrl = "http://localhost:5016";
    private const string FirebaseApiKey = "AIzaSyDxAIVCgS8yuHnqHbUaWgB2AM1_0gaikqo";
    private const string AdminEmail = "admin@admin.com";
    private const string AdminPassword = "Sifra123";
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
        await Register(AdminEmail);
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

        var userToken = await GetFirebaseTokenAsync(_email, TestPassword);
        var mongoUserId = await GetCurrentUserIdAsync(userToken);

        if (mongoUserId is not null)
        {
            var adminToken = await GetFirebaseTokenAsync(AdminEmail, AdminPassword);
            var adminContext = await CreateApiContextAsync(adminToken);
            await adminContext.DeleteAsync($"user/{mongoUserId}");
            await adminContext.DisposeAsync();
        }

        _email = string.Empty;
    }

    private async Task Register(string email, string? password = null)
    {
        await FillForm("John", "Doe", email, password ?? TestPassword);
        await SignUpButton.ClickAsync();
    }

    private async Task<string> GetFirebaseTokenAsync(string email, string password)
    {
        var firebaseContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://identitytoolkit.googleapis.com/"
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

    private async Task<IAPIRequestContext> CreateApiContextAsync(string token) =>
        await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"{BackendUrl}/",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            }
        });

    private async Task<string?> GetCurrentUserIdAsync(string token)
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

    private async Task FillForm(string name, string surname, string email, string password)
    {
        await NameInput.FillAsync(name);
        await SurnameInput.FillAsync(surname);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
    }
}
