using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public interface ICloudinaryRepository
{
    string BuildImageUrl(string publicId);
    Task<ImageUploadResult> UploadProductImageAsync(string productId, string fileName, Stream payload);
    Task<DeletionResult> DeleteProductImage(string publicId);
}