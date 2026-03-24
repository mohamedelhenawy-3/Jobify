using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.FileUploadService
{
    public class SupabaseService
    {
        private readonly SupabaseSettings _settings;
        public SupabaseService(IOptions<SupabaseSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> UploadResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            var fileName = Guid.NewGuid() + ".pdf";

            // Upload URL for authenticated upload
            var uploadUrl = $"{_settings.Url}/storage/v1/object/{_settings.Bucket}/{fileName}";

            using var client = new HttpClient();

            // Required headers
            client.DefaultRequestHeaders.Add("apikey", _settings.ApiKey);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

            using var content = new MultipartFormDataContent();

            // Set content-type to application/pdf
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            // "file" must match Supabase expectation
            content.Add(streamContent, "file", fileName);

            // Send POST request
            var response = await client.PostAsync(uploadUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed. Status: {response.StatusCode}, Body: {body}");
            }

            // Return public URL for browser access
            return $"{_settings.Url}/storage/v1/object/public/{_settings.Bucket}/{fileName}";
        }
    }
}