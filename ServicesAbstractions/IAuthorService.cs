using Shared;
using Shared.Dtos.Author;

namespace ServicesAbstractions
{
    public interface IAuthorService
    {
        Task<ApiResponse<string>> CreateAuthorAsync(CreateAuthorDto dto);
        Task<ApiResponse<string>> UpdateAuthorAsync(int authorId, UpdateAuthorDto dto);
        Task<ApiResponse<string>> DeleteAuthorAsync(int authorId, string deletedBy);
        Task<ApiResponse<AuthorResponseDto>> GetAuthorByIdAsync(int authorId);
        Task<ApiResponse<PaginatedResponse<AuthorResponseDto>>> GetAllAuthorsAsync(AuthorSearchFilterDto filterDto);
    }
}