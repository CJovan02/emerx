using CloudinaryDotNet.Actions;

namespace EMerx.Repositories.CloudinaryRepository;

public interface ICloudinaryRepository
{
    Task<ImageUploadResult> UploadProductImageAsync(string productId, string fileName, Stream payload);
    Task<DeletionResult> DeleteProductImage(string publicId);
}