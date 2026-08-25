using Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServicesAbstractions;


namespace Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webRootPath;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private const int MaxFilesPerRequest = 10;

        public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            _httpContextAccessor = httpContextAccessor;
        } 
        public async Task<List<string>> SaveFilesAsync(IFormFileCollection files, string subFolder)
        {
            var urls = new List<string>();
            var uploadsRoot = Path.Combine(_webRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsRoot);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                urls.Add($"/uploads/{subFolder}/{fileName}");
            }

            return urls;
        }

        public void DeleteFiles(List<string> relativeUrls)
        {
            foreach (var relativeUrl in relativeUrls)
            {
                var fullPath = Path.Combine(_webRootPath, relativeUrl.TrimStart('/'));
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
        }

        public void Validate(IFormFileCollection images)
        {
            if (images is null || images.Count == 0)
                throw new BadRequestException("At least one image is required");

            if (images.Count > MaxFilesPerRequest)
                throw new BadRequestException($"Cannot upload more than {MaxFilesPerRequest} images at once");

            foreach (var file in images)
            {
                if (file.Length == 0)
                    throw new BadRequestException($"File '{file.FileName}' is empty");

                if (file.Length > MaxFileSizeBytes)
                    throw new BadRequestException($"File '{file.FileName}' exceeds the {MaxFileSizeBytes / (1024 * 1024)}MB limit");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                    throw new BadRequestException($"File '{file.FileName}' has an unsupported extension. Allowed: {string.Join(", ", AllowedExtensions)}");

                if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                    throw new BadRequestException($"File '{file.FileName}' has an unsupported content type '{file.ContentType}'");
            }
        }

        public string BuildAbsoluteUrl(string relativeUrl)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is null || string.IsNullOrWhiteSpace(relativeUrl)) return relativeUrl;

            return $"{request.Scheme}://{request.Host}{relativeUrl}";
        }
    }
}
