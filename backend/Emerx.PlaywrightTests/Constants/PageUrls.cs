namespace Emerx.PlaywrightTests.Constants;

public static class PageUrls
{

    public static string ServerUrl => "http://localhost:5016";
    public static string BaseUrl => "http://localhost:5173";

    public static string LoginPage => $"{BaseUrl}/login";
    public static string ProductsPage => $"{BaseUrl}/products";
    public static string ProductsPageRaw => "/products";

    public static string AdminsManagementPage => $"{BaseUrl}/admin/admins-management";
}