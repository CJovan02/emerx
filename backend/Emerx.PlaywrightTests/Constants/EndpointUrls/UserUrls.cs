namespace Emerx.PlaywrightTests.Constants;

public static class UserUrls
{
    public static string Base => "/User";
    public static string Register => $"{Base}/register";
    public static string GrantAdminRole => $"{Base}/grantAdminRole";
    public static string RemoveAdminRole => $"{Base}/removeAdminRole";
}