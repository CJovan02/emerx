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
            { "Test-Auth", "true" },
        };

        HttpClient = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = PageUrls.BaseUrl,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true
        });
    }

    [Test]
    public async Task GetCategories()
    {
        await using var response = await HttpClient.GetAsync("/Products/categories");
        Console.WriteLine($"Status: {response.Status}");
        Console.WriteLine($"Status text: {response.StatusText}");

        var body = await response.TextAsync();

        Console.WriteLine(body);
    }
}