using CloudinaryDotNet;
using Microsoft.Extensions.Options;

namespace EMerx.Infrastructure.CloudinaryContext;

public class CloudinaryContext(IOptions<CloudinarySettings> options, ILogger<CloudinaryContext> logger)
{
    // I expose the entire client object because I'm lazy to manually expose all the function I would use in project :)
    public Cloudinary Client { get; private set; }

    public void Connect()
    {
        var settings = options.Value;
        var account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret
        );
        Client = new Cloudinary(account)
        {
            Api =
            {
                Secure = true
            }
        };
    }

    public async Task PingAsync()
    {
        logger.LogInformation("Pinging Cloudinary connection...");
        try
        {
            await Client.PingAsync();
            logger.LogInformation("✅ Successfully pinged Cloudinary connection.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Cloudinary ping failed.");
            throw ex;
        }
    }
}