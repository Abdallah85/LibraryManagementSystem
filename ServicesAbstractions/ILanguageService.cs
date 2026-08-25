using Shared;
using Shared.Dtos.Language;

namespace ServicesAbstractions
{
    public interface ILanguageService
    {
        Task<ApiResponse<string>> CreateLanguageAsync(CreateLanguageDto dto);
        Task<ApiResponse<string>> UpdateLanguageAsync(int languageId, UpdateLanguageDto dto);
        Task<ApiResponse<string>> DeleteLanguageAsync(int languageId, string? deletedBy);
        Task<ApiResponse<LanguageResponseDto>> GetLanguageByIdAsync(int languageId);
        Task<ApiResponse<PaginatedResponse<LanguageResponseDto>>> GetAllLanguagesAsync(LanguageSearchFilterDto filterDto);
    }
}