using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductDetailsPage : PageTest
{
    private const string BaseUrl = "http://localhost:5173";
    private const string BackendUrl = "http://localhost:5016";
    private const string ProductId = "69a18f932a241fbd4433dd3b";
    private const string MockUserPassword = "MockTest123!";

    private static readonly HttpClient Http = new();

    private string _mockUserEmail = "";
    private string _idToken = "";

    [SetUp]
    public async Task SetUpAuthenticated()
    {
        _mockUserEmail = $"playwright-{Guid.NewGuid():N}@test.com";

        var registerPayload = JsonSerializer.Serialize(new
        {
            name = "Playwright",
            surname = "Test",
            email = _mockUserEmail,
            password = MockUserPassword
        });
        await Http.PostAsync(
            $"{BackendUrl}/User/register",
            new StringContent(registerPayload, Encoding.UTF8, "application/json"));

        await Page.RouteAsync("**/api/**", async route =>
        {
            if (string.IsNullOrEmpty(_idToken))
            {
                var authHeader = route.Request.Headers
                    .FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Value ?? "";
                if (authHeader.StartsWith("Bearer "))
                    _idToken = authHeader["Bearer ".Length..];
            }
            await route.ContinueAsync();
        });

        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.Locator("#email").FillAsync(_mockUserEmail);
        await Page.Locator("#password").FillAsync(MockUserPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/products", new() { Timeout = 10000 });
        await Page.GotoAsync($"{BaseUrl}/products/{ProductId}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });
    }

    [TearDown]
    public async Task DeleteMockUser()
    {
        if (string.IsNullOrEmpty(_idToken)) return;

        // userId in ReviewResponse is the MongoDB ObjectId, not the Firebase UID —
        // resolve it via GET /User (returns the logged-in user's MongoDB profile)
        var userReq = new HttpRequestMessage(HttpMethod.Get, $"{BackendUrl}/User");
        userReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
        var userResp = await Http.SendAsync(userReq);

        if (userResp.IsSuccessStatusCode)
        {
            var userJson = JsonDocument.Parse(await userResp.Content.ReadAsStringAsync());
            var mongoUserId = userJson.RootElement.GetProperty("id").GetString();

            var reviewsReq = new HttpRequestMessage(HttpMethod.Get,
                $"{BackendUrl}/Review/getProductReviews/{ProductId}");
            reviewsReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
            var reviewsResp = await Http.SendAsync(reviewsReq);

            if (reviewsResp.IsSuccessStatusCode)
            {
                var reviews = JsonDocument.Parse(await reviewsResp.Content.ReadAsStringAsync());
                foreach (var review in reviews.RootElement.EnumerateArray())
                {
                    if (review.GetProperty("userId").GetString() != mongoUserId) continue;

                    var reviewId = review.GetProperty("id").GetString();
                    var deleteReviewReq = new HttpRequestMessage(HttpMethod.Delete,
                        $"{BackendUrl}/Review/{reviewId}");
                    deleteReviewReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
                    await Http.SendAsync(deleteReviewReq);
                    break;
                }
            }
        }

        var deleteUserReq = new HttpRequestMessage(HttpMethod.Delete, $"{BackendUrl}/User");
        deleteUserReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
        await Http.SendAsync(deleteUserReq);
    }

    private ILocator LoadingSpinner => Page.Locator(".MuiCircularProgress-root");
    private ILocator AddToCartButton => Page.GetByRole(AriaRole.Button, new() { Name = "Add to Cart" });
    private ILocator StepperControls => Page.GetByText("Item Quantity").Locator("xpath=following-sibling::div");
    private ILocator DecrementButton => StepperControls.GetByRole(AriaRole.Button).First;
    private ILocator IncrementButton => StepperControls.GetByRole(AriaRole.Button).Last;
    private ILocator QuantityDisplay => StepperControls.Locator("p");
    private ILocator SuccessSnackbar => Page.Locator("[class*='notistack-MuiContent-success']");
    private ILocator ErrorSnackbar => Page.Locator("[class*='notistack-MuiContent-error']");

    // Review form — only rendered when the logged-in user has not yet reviewed this product
    private ILocator WriteReviewForm => Page.Locator("form").Filter(new() { HasText = "Write a Review" });
    private ILocator ReviewDescription => WriteReviewForm.GetByLabel("Description");
    private ILocator SubmitReviewButton => WriteReviewForm.GetByRole(AriaRole.Button, new() { Name = "Submit Review" });
    private ILocator RatingValidationError => WriteReviewForm.GetByText("Please select a rating");
    private ILocator DescriptionValidationError => WriteReviewForm.GetByText("Description is required");

    private async Task WaitForReviewsToLoad()
    {
        // "Customer Reviews" heading only renders after auth completes and the product page mounts.
        // This covers the race where Firebase fires onAuthStateChanged from IndexedDB after
        // NetworkIdle has already been satisfied (no network during IndexedDB reads).
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Customer Reviews" })).ToBeVisibleAsync(new() { Timeout = 10000 });
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
    }

    [Test]
    public async Task ProductDetails_PageLoads_ShowsProductName()
    {
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 4 })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ProductDetails_PageLoads_ShowsCategoryChipAndPrice()
    {
        await Expect(Page.Locator(".MuiChip-root")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 5 })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ProductDetails_AddToCart_ShowsSuccessSnackbar()
    {
        Assume.That(await AddToCartButton.IsEnabledAsync(), "Product is out of stock");

        await AddToCartButton.ClickAsync();

        await Expect(SuccessSnackbar).ToBeVisibleAsync(new() { Timeout = 5000 });
    }

    [Test]
    public async Task ProductDetails_IncrementQuantity_UpdatesDisplay()
    {
        Assume.That(await IncrementButton.IsEnabledAsync(), "Only 1 item in stock, cannot increment");

        await IncrementButton.ClickAsync();

        await Expect(QuantityDisplay).ToHaveTextAsync("2");
    }

    [Test]
    public async Task ProductDetails_DecrementAtOne_IsDisabled()
    {
        await Expect(DecrementButton).ToBeDisabledAsync();
    }

    [Test]
    public async Task ProductDetails_ReviewsSection_Loads()
    {
        var noReviews = Page.GetByText("No reviews yet.");
        var firstReviewCard = Page.Locator(".MuiCard-root").First;

        await Expect(noReviews.Or(firstReviewCard)).ToBeVisibleAsync(new() { Timeout = 10000 });
    }

    [Test]
    public async Task Review_SubmitWithoutRating_ShowsValidationError()
    {
        // my profile
        await WaitForReviewsToLoad();
        Assume.That(await WriteReviewForm.IsVisibleAsync(), "User has already reviewed this product");

        await ReviewDescription.FillAsync("Great product!");
        await SubmitReviewButton.ClickAsync();

        await Expect(RatingValidationError).ToBeVisibleAsync();
    }

    [Test]
    public async Task Review_SubmitWithoutDescription_ShowsValidationError()
    {
        await WaitForReviewsToLoad();
        Assume.That(await WriteReviewForm.IsVisibleAsync(), "User has already reviewed this product");

        await WriteReviewForm.GetByRole(AriaRole.Radio).Nth(2).ClickAsync(); // 3 stars
        await SubmitReviewButton.ClickAsync();

        await Expect(DescriptionValidationError).ToBeVisibleAsync();
    }

    [Test]
    public async Task Review_FillThenClearDescription_ShowsValidationErrorOnSubmit()
    {
        await WaitForReviewsToLoad();
        Assume.That(await WriteReviewForm.IsVisibleAsync(), "User has already reviewed this product");

        await WriteReviewForm.GetByRole(AriaRole.Radio).Nth(2).ClickAsync();
        await ReviewDescription.FillAsync("Some text I changed my mind about");
        await ReviewDescription.FillAsync("");

        await SubmitReviewButton.ClickAsync();

        await Expect(DescriptionValidationError).ToBeVisibleAsync();
    }

    [Test]
    public async Task Review_SubmitValidReview_ShowsApiResponse()
    {
        await WaitForReviewsToLoad();
        Assume.That(await WriteReviewForm.IsVisibleAsync(), "User has already reviewed this product");

        await WriteReviewForm.GetByRole(AriaRole.Radio).Nth(2).ClickAsync(); // 3 stars
        await ReviewDescription.FillAsync("This is a great product, highly recommend!");

        await SubmitReviewButton.ClickAsync();

        // Outcome depends on whether the user has ordered this product:
        // - success →  disappears and review appears in the list
        // - 403     → "You need to order this product before leaving a review."
        // - 409     → "You already posted a review for this product."
        await Expect(SuccessSnackbar.Or(ErrorSnackbar)).ToBeVisibleAsync(new() { Timeout = 10000 });
    }

    [Test]
    public async Task ProductDetails_InvalidId_ShowsNotFoundError()
    {
        await Page.GotoAsync($"{BaseUrl}/products/000000000000000000000000");
        await Expect(LoadingSpinner).Not.ToBeVisibleAsync(new() { Timeout = 10000 });

        await Expect(Page.GetByText("Product not found")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Go to products" })).ToBeVisibleAsync();
    }
}
