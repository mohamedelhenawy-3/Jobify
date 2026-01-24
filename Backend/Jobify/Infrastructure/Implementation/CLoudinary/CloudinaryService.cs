using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.CLoudinary
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                  config["Cloudinary:CloudName"],
                  config["Cloudinary:ApiKey"],
                  config["Cloudinary:ApiSecret"]
              );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        // Upload image and return URL + PublicId
        public async Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file provided or file is empty");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "company_profiles"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result == null)
                throw new Exception("Cloudinary returned null result");

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            if (string.IsNullOrEmpty(result.SecureUrl?.ToString()))
                throw new Exception("Cloudinary upload succeeded but SecureUrl is null");

            return (result.SecureUrl.ToString(), result.PublicId);
        }

        // Delete image by PublicId
        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) return;

            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}
