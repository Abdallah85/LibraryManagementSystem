using Microsoft.AspNetCore.Http;

namespace ServicesAbstractions
{
    public interface IFileStorageService
    {
        Task<List<string>> SaveFilesAsync(IFormFileCollection files, string subFolder);
        void DeleteFiles(List<string> relativeUrls);
        void Validate(IFormFileCollection images);
        string BuildAbsoluteUrl(string relativeUrl);
    }
}
