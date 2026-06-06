namespace Emerx.PlaywrightTests;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void Setup()
    {
        var root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../")
        );

        var envPath = Path.Combine(root, "src", ".env.local");

        Console.WriteLine(envPath);

        DotNetEnv.Env.Load(envPath);
    }
}