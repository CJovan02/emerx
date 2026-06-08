namespace Emerx.PlaywrightTests.Constants;

public static class PageUrls
{
    private static string BaseUrl => ServerUrls.Frontend;
    public static string FirebaseTokenUrl => "https://identitytoolkit.googleapis.com/";
    public static string AdminsManagementPage => $"{BaseUrl}/admin/admins-management";

    public static string LoginPage => $"{BaseUrl}/login";
    public static string ProductsPage => $"{BaseUrl}/products";
    public static string ProductsPageRaw => "/products";

    public static string AdminProductPage => $"{BaseUrl}/admin/products";
}