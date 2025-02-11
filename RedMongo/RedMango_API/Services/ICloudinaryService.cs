using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RedMango_API.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder);
        string GetImageUrl(string publicId);
        Task<DeletionResult> DeleteImageAsync(string publicId);

        string GetPublicId(string url);
    }
}
