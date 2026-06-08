using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EMerx.DTOs.Users.Response;
using Emerx.PlaywrightTests.Helpers;
using Emerx.PlaywrightTests.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProfilePage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";
    private const string BackendUrl = "http://localhost:5016";
    private const string MockUserPassword = "MockTest123!";

    private static readonly HttpClient Http = new();

    private string _mockUserEmail = "";
    private string _idToken = "";
    private UserResponse _user;
    private BackendApiService _api;

    [SetUp]
    public async Task SetUpAuthenticated()
    {
        _api = new BackendApiService(Playwright);
        _mockUserEmail = $"playwright-{Guid.NewGuid():N}@test.com";
        // We don't authorize here since we first need to register new account
        await _api.ConnectAsync("");


        _user = await _api.RegisterUserAsync(_mockUserEmail, MockUserPassword, "Playwright", "Test");
        _idToken = await AuthHelper.GetFirebaseTokenAsync(Playwright, _mockUserEmail, MockUserPassword);

        // After creating new account we authorize the _api with the new user
        await _api.ConnectAsync(_idToken);

        // var registerPayload = JsonSerializer.Serialize(new
        // {
        //     name = "Playwright",
        //     surname = "Test",
        //     email = _mockUserEmail,
        //     password = MockUserPassword
        // });
        // await Http.PostAsync(
        //     $"{BackendUrl}/User/register",
        //     new StringContent(registerPayload, Encoding.UTF8, "application/json"));
        //
        // await Page.RouteAsync("**/api/**", async route =>
        // {
        //     if (string.IsNullOrEmpty(_idToken))
        //     {
        //         var authHeader = route.Request.Headers
        //             .FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Value ?? "";
        //         if (authHeader.StartsWith("Bearer "))
        //             _idToken = authHeader["Bearer ".Length..];
        //     }
        //     await route.ContinueAsync();
        // });
        // await _api.ConnectAsync(_idToken);

        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.Locator("#email").FillAsync(_mockUserEmail);
        await Page.Locator("#password").FillAsync(MockUserPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/products", new() { Timeout = 10000 });

        await Page.GotoAsync($"{BaseUrl}/my-profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });
    }

    [TearDown]
    public async Task DeleteMockUser()
    {
        if (string.IsNullOrEmpty(_idToken)) return;

        await _api.DeleteUserSelfAsync();
        await _api.DisposeAsync();

        // var deleteUserReq = new HttpRequestMessage(HttpMethod.Delete, $"{BackendUrl}/User");
        // deleteUserReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
        // await Http.SendAsync(deleteUserReq);
    }

    private ILocator EditButton => Page.GetByRole(AriaRole.Button, new() { Name = "Edit" });
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });
    private ILocator CancelButton => Page.GetByRole(AriaRole.Button, new() { Name = "Cancel" });
    private ILocator NameField => Page.GetByLabel("Name", new() { Exact = true });
    private ILocator SurnameField => Page.GetByLabel("Surname", new() { Exact = true });
    private ILocator CityField => Page.GetByLabel("City");
    private ILocator StreetField => Page.GetByLabel("Street");
    private ILocator HouseNumberField => Page.GetByLabel("House Number");

    [Test]
    public async Task Profile_PageLoads_ShowsUserData()
    {
        await Expect(Page.GetByText("Playwright Test")).ToBeVisibleAsync();
        await Expect(Page.GetByText(_mockUserEmail)).ToBeVisibleAsync();
    }

    [Test]
    public async Task Profile_ClickEdit_ShowsEditForm()
    {
        await EditButton.ClickAsync();

        await Expect(NameField).ToBeVisibleAsync();
        await Expect(SurnameField).ToBeVisibleAsync();
        await Expect(SaveButton).ToBeVisibleAsync();
        await Expect(CancelButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task Profile_ClickCancel_RestoresViewMode()
    {
        await EditButton.ClickAsync();
        await CancelButton.ClickAsync();

        await Expect(EditButton).ToBeVisibleAsync();
        await Expect(SaveButton).Not.ToBeVisibleAsync();
        await Expect(Page.GetByText("Playwright Test")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Profile_EditName_UpdatesDisplayedName()
    {
        await EditButton.ClickAsync();

        await NameField.FillAsync("Updated");
        await SurnameField.FillAsync("User");
        await SaveButton.ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });

        await Expect(EditButton).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.GetByText("Updated User")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Profile_EditAddress_UpdatesDisplayedAddress()
    {
        await EditButton.ClickAsync();

        await CityField.FillAsync("Belgrade");
        await StreetField.FillAsync("Knez Mihailova");
        await HouseNumberField.FillAsync("10");
        await SaveButton.ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });

        await Expect(EditButton).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.GetByText("Belgrade")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Knez Mihailova")).ToBeVisibleAsync();
        await Expect(Page.GetByText("10")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Profile_EditCancel_DoesNotPersistChanges()
    {
        await EditButton.ClickAsync();

        await NameField.FillAsync("ShouldNotSave");
        await CancelButton.ClickAsync();

        await Expect(Page.GetByText("Playwright Test")).ToBeVisibleAsync();
        await Expect(Page.GetByText("ShouldNotSave")).Not.ToBeVisibleAsync();
    }
}
