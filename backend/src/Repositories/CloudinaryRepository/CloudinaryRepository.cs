using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EMerx.Infrastructure.CloudinaryContext;

namespace EMerx.Repositories.CloudinaryRepository;

public class CloudinaryRepository(CloudinaryContext cloudinaryContext) : ICloudinaryRepository
{
    private readonly Cloudinary _client = cloudinaryContext.Client;

    public string BuildImageUrl(string publicId)
    {
        return _client.Api.UrlImgUp.Secure(true).BuildUrl(publicId);
    }

    public async Task<ImageUploadResult> UploadProductImageAsync(string productId, string fileName, Stream payload)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, payload),
            Folder = $"emerx/products/{productId}",
            PublicId = Guid.NewGuid().ToString(),
            Overwrite = false,
        };

        var result = await _client.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }

    public async Task<DeletionResult> DeleteProductImage(string publicId)
    {
        var result = await _client.DestroyAsync(new DeletionParams(publicId));
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }
}