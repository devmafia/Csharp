using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RedMango_API.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("CloudinarySettings");
            string cloudName = cloudinarySettings["CloudName"];
            string apiKey = cloudinarySettings["ApiKey"];
            string apiSecret = cloudinarySettings["ApiSecret"];

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream), AssetFolder = "Images",
                    PublicId = fileName,
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };

                return await _cloudinary.UploadAsync(uploadParams);
            }
        }

        public string GetImageUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentException("Public ID must not be null or empty.");
            }

            return _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentException("Public ID must not be null or empty.");
            }

            var deletionParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deletionParams);
        }

        public string GetPublicId(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Folder and fileName cannot be null or empty.");
            }
          fileName = Path.GetFileNameWithoutExtension(fileName).Trim();

            return fileName;
        }

    }
}
