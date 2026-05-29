using EMerx.Common.Filters;
using EMerx.DTOs.Products.Response;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.ApiTests;

[TestFixture]
public class ProductsApiTest : PlaywrightTest
{
    private IAPIRequestContext HttpClient;

    [SetUp]
    public async Task SetUp()
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };

        HttpClient = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ServerUrls.BackendTest,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true
        });
    }

    [Test]
    public async Task GetPaged()
    {
        // Arrange
        const int page = 5;
        const int pageSize = 5;

        // Act
        await using var response =
            await HttpClient.GetAsync($"{ProductUrls.GetPaged}?Page={page}&PageSize={pageSize}");

        // Assert
        var pageResponse = await response.JsonAsync<PageOfResponse<ProductResponse>>();
        Assert.Multiple(() =>
        {
            Assert.That(response.Status,  Is.EqualTo(200));
            Assert.That(pageResponse, Is.Not.Null);
            Assert.That(pageResponse.Page, Is.EqualTo(page));
            Assert.That(pageResponse.PageSize, Is.EqualTo(pageSize));
            // If there are not enough items for the specific page and pageSize, server may return fewer items than specified
            Assert.That(pageResponse.Items.Count, Is.LessThanOrEqualTo(page * pageSize));
        });
    }

    [Test]
    public async Task GetCategories()
    {
        // Arrange, Act
        await using var response = await HttpClient.GetAsync(ProductUrls.GetCategories);

        // Assert
        var categories = await response.JsonAsync<List<string>>();

        Assert.Multiple(() =>
        {
            Assert.That(response.Status, Is.EqualTo(200));
            Assert.That(categories, Is.Not.Empty);
            Assert.That(categories, Is.Not.Null);
        });
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await HttpClient.DisposeAsync();
    }
}