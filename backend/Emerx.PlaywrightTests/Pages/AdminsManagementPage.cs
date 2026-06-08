using EMerx.Auth;
using Emerx.PlaywrightTests.Constants;
using Emerx.PlaywrightTests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AdminsManagementPage : PageTest
{
    private ILocator EmailField => Page.GetByLabel("Email");
    private ILocator GrantButton => Page.GetByTestId("grant-admin-button");
    private ILocator RemoveButton => Page.GetByTestId("remove-admin-button");
    private ILocator GrantSuccessSnackbar => Page.GetByText("Successfully granted admin role.");
    private ILocator RemoveSuccessSnackbar => Page.GetByText("Successfully removed admin role.");
    private ILocator CannotEditOwnRolesSnackbar => Page.GetByText("You can't edit your own roles.");

    [SetUp]
    public async Task SetupAsAdmin()
    {
        await AuthHelper.LoginAsAdmin(Page);

        await Page.GotoAsync(PageUrls.AdminsManagementPage);
        // await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });
        await EmailField.WaitForAsync();
    }

    [Test]
    public async Task CanGrantAdminRole()
    {
        await EmailField.FillAsync(TestUsers.User.Email);
        await GrantButton.ClickAsync();

        await Expect(GrantSuccessSnackbar).ToBeVisibleAsync();

        // cleanup
        await RemoveButton.ClickAsync();
    }

    [Test]
    public async Task CanRemoveAdminRole()
    {
        await EmailField.FillAsync(TestUsers.User.Email);
        await RemoveButton.ClickAsync();

        await Expect(RemoveSuccessSnackbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task EnteredCurrentUser_CannotGrant()
    {
        await EmailField.FillAsync(AuthHelper.AdminEmail);
        await GrantButton.ClickAsync();

        await Expect(CannotEditOwnRolesSnackbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task EnteredCurrentUser_CannotRemove()
    {
        await EmailField.FillAsync(AuthHelper.AdminEmail);
        await RemoveButton.ClickAsync();

        await Expect(CannotEditOwnRolesSnackbar).ToBeVisibleAsync();
    }
}