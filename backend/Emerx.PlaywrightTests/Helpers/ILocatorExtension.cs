using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.Helpers;

public static class ILocatorExtension
{
    /// <summary>
    /// Works best with elements that are a part of a form.
    /// </summary>
    public static async Task<bool> CheckFieldValidity(this ILocator locator)
    {
        return await locator
            .EvaluateAsync<bool>("el => el.checkValidity()");
    }
}