using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public interface ICloudinaryRepository
{
    string BuildImageUrl(string publicId);
    string BuildProductThumbnailImageUrl(string productId);
    Task<ImageUploadResult> UploadProductThumbnailAsync(string productId, Stream payload, bool overwrite = false);
    Task<DeletionResult> DeleteProductThumbnail(string productId);
    Task<DeleteFolderResult> DeleteProductFolder(string productId);
}