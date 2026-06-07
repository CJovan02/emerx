using System.Text.Json;

namespace Emerx.PlaywrightTests.Common;

public static class JsonSerializers
{
    public static JsonSerializerOptions CaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}