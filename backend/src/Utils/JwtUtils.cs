namespace EMerx.Utils;

public static class JwtUtils
{
    public static string? GetUidFromHttpContext(HttpContext httpContext)
    {
        return httpContext.User.FindFirst("user_id")?.Value;
    }
}