namespace Emerx.PlaywrightTests.Constants;

public static class ApiTestUrls
{
    public static string Orders => $"{ServerUrls.BackendTest}/Order/";
    public static string OrderOverviewRaw => "overview";

    public static string Products => $"{ServerUrls.BackendTest}/Product/";
    public static string ProductCategoriesRaw => "categories";
}