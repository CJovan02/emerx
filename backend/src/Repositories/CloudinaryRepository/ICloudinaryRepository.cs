using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public interface ICloudinaryRepository
{
    string BuildImageUrl(string publicId);
    string BuildProductThumbnailImageUrl(string productId);
    Task<ImageUploadResult> UploadProductThumbnailAsync(string productId, Stream payload);
    Task<DeletionResult> DeleteProductImage(string publicId);
    Task<DeleteFolderResult> DeleteProductFolder(string productId);
}