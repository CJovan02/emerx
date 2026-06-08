using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductsPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";

    [SetUp]
    public async Task SetUpAuthenticated()
    {
        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.Locator("#email").FillAsync("atest@test.com");
        await Page.Locator("#password").FillAsync("Test123!");
        await Page.Locator("button[type='submit']").ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/products", new() { Timeout = 10000 });
    }

    private ILocator SearchInput => Page.GetByPlaceholder("Search products…");
    private ILocator ProductCards => Page.Locator(".MuiCardActionArea-root");
    private ILocator Skeletons => Page.Locator(".MuiSkeleton-root");
    private ILocator EmptyMessage => Page.GetByText("No products found.");
    private ILocator InStockSwitch => Page.GetByLabel("In stock only");
    private ILocator ClearFiltersButton => Page.GetByRole(AriaRole.Button, new() { Name = "Clear all" });

    private async Task WaitForLoadingToFinish() =>
        await Expect(Skeletons.First).Not.ToBeVisibleAsync(new() { Timeout = 10000 });

    private async Task WaitForProductsOrEmpty() =>
        await Expect(ProductCards.First.Or(EmptyMessage)).ToBeVisibleAsync(new() { Timeout = 10000 });

    [Test]
    public async Task Products_Search_ReloadsResults()
    {
        await WaitForLoadingToFinish();

        var responseTask = Page.WaitForResponseAsync(resp => resp.Url.Contains("Product"));
        await SearchInput.FillAsync("a");
        await responseTask;

        await WaitForLoadingToFinish();
    }

    [Test]
    public async Task Products_ClickCard_NavigatesToProductDetails()
    {
        await WaitForProductsOrEmpty();

        var firstCard = ProductCards.First;
        Assume.That(await firstCard.IsVisibleAsync(), "No products available to click");

        await firstCard.ClickAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(BaseUrl)}/products/[a-zA-Z0-9]+"),
            new() { Timeout = 5000 });
    }

    [Test]
    public async Task Products_InStockOnly_FilterIsApplied()
    {
        await InStockSwitch.ClickAsync();

        await Expect(InStockSwitch).ToBeCheckedAsync();
        await WaitForLoadingToFinish();
    }

    [Test]
    public async Task Products_ClearFilters_ResetsFilters()
    {
        await InStockSwitch.ClickAsync();
        await Expect(ClearFiltersButton).ToBeVisibleAsync();

        await ClearFiltersButton.ClickAsync();

        await Expect(InStockSwitch).Not.ToBeCheckedAsync();
        await Expect(ClearFiltersButton).Not.ToBeVisibleAsync();
    }
}
