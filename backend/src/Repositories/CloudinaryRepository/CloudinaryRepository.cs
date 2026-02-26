using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EMerx.Infrastructure.CloudinaryContext;

namespace EMerx.Repositories.CloudinaryRepository;

public class CloudinaryRepository(CloudinaryContext cloudinaryContext) : ICloudinaryRepository
{
    private readonly Cloudinary _client = cloudinaryContext.Client;

    private static string GenerateProductFolder(string productId)
    {
        return $"emerx/products/{productId}";
    }

    private static string GenerateProductThumbnailPublicId(string productId)
    {
        return $"products/{productId}/thumbnail";
    }

    public string BuildImageUrl(string publicId, string version)
    {
        return _client.Api.UrlImgUp.Secure(true).Version(version).BuildUrl(publicId);
    }

    public string BuildProductThumbnailImageUrl(string productId, string version)
    {
        return BuildImageUrl(GenerateProductThumbnailPublicId(productId), version);
    }

    public async Task<ImageUploadResult> UploadProductThumbnailAsync(string productId, Stream payload,
        bool overwrite = false)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription("thumbnail", payload),
            AssetFolder = GenerateProductFolder(productId),
            PublicId = GenerateProductThumbnailPublicId(productId),
            Overwrite = overwrite,
            Invalidate = overwrite
        };

        var result = await _client.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }

    public async Task<DeletionResult> DeleteProductThumbnail(string productId)
    {
        var publicId = GenerateProductThumbnailPublicId(productId);
        var result = await _client.DestroyAsync(new DeletionParams(publicId));
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }

    public async Task<DeleteFolderResult> DeleteProductFolder(string productId)
    {
        var result = await _client.DeleteFolderAsync(GenerateProductFolder(productId));
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result;
    }
}