using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public interface ICloudinaryRepository
{
    string BuildImageUrl(string publicId, string version);
    string BuildProductThumbnailImageUrl(string productId, string version);
    Task<ImageUploadResult> UploadProductThumbnailAsync(string productId, Stream payload, bool overwrite = false);
    Task<DeletionResult> DeleteProductThumbnail(string productId);
    Task<DelResResult> DeleteProductImages(string productId);
    Task<DeleteFolderResult> DeleteProductFolder(string productId);
}