namespace EMerx.Utils;

public static class PasswordUtils
{
    public static bool HasUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    public static bool HasLowercase(string password)
    {
        return password.Any(char.IsLower);
    }

    public static bool IsLongEnough(string password, int length = 6)
    {
        return password.Length >= length;
    }
}