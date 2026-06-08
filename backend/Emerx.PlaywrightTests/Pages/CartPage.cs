using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CartPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";
    private const string ProductId = "69a18f932a241fbd4433dd3b";

    [SetUp]
    public async Task SetUpWithItemInCart()
    {
        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.Locator("#email").FillAsync("atest@test.com");
        await Page.Locator("#password").FillAsync("Test123!");
        await Page.Locator("button[type='submit']").ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/products", new() { Timeout = 10000 });

        await Page.GotoAsync($"{BaseUrl}/products/{ProductId}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });

        var addToCart = Page.GetByRole(AriaRole.Button, new() { Name = "Add to Cart" });
        Assume.That(await addToCart.IsEnabledAsync(), "Product is out of stock");
        await addToCart.ClickAsync();
    }

    private ILocator CartButton => Page.GetByRole(AriaRole.Button, new() { Name = "cart", Exact = true });
    private ILocator CartMenu => Page.Locator("#cart-menu");
    private ILocator CheckoutButton => CartMenu.GetByRole(AriaRole.Button, new() { Name = "Checkout" });
    private ILocator RemoveItemButton => CartMenu.Locator(".MuiIconButton-root").First;
    // First body2 Typography in the menu — the product name inside the item row
    private ILocator FirstCartItemName => CartMenu.Locator(".MuiTypography-body2").First;

    [Test]
    public async Task Cart_OpenCart_ShowsMenuWithSubtotal()
    {
        await CartButton.ClickAsync();

        await Expect(CartMenu).ToBeVisibleAsync();
        await Expect(CartMenu.GetByText("Subtotal")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Cart_WithItem_CheckoutButtonIsEnabled()
    {
        await CartButton.ClickAsync();

        await Expect(CheckoutButton).ToBeEnabledAsync();
    }

    [Test]
    public async Task Cart_ClickCheckout_NavigatesToCheckoutPage()
    {
        await CartButton.ClickAsync();
        await CheckoutButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/checkout");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 4 })).ToHaveTextAsync("Checkout");
    }

    [Test]
    public async Task Cart_CheckoutPage_ShowsShippingFormAndCartSummary()
    {
        await CartButton.ClickAsync();
        await CheckoutButton.ClickAsync();

        await Expect(Page.GetByText("Shipping Information")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Review your cart")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Continue to Review" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Cart_ClickItem_NavigatesToProductDetailsPage()
    {
        await CartButton.ClickAsync();

        await FirstCartItemName.ClickAsync();

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(BaseUrl)}/products/[a-zA-Z0-9]+"),
            new() { Timeout = 5000 });
    }

    [Test]
    public async Task Cart_RemoveItem_CheckoutButtonBecomesDisabled()
    {
        await CartButton.ClickAsync();
        await RemoveItemButton.ClickAsync();

        await Expect(CheckoutButton).ToBeDisabledAsync();
    }
}
