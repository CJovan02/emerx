using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public class CloudinaryRepository(Cloudinary client) : ICloudinaryRepository
{
    public async Task<ImageUploadResult> UploadProductImageAsync(string productId, string fileName, Stream payload)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, payload),
            Folder = $"emerx/products/{productId}",
            PublicId = Guid.NewGuid().ToString(),
            Overwrite = false,
        };

        var result = await client.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }

    public async Task<DeletionResult> DeleteProductImage(string publicId)
    {
        var result = await client.DestroyAsync(new DeletionParams(publicId));
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }
}