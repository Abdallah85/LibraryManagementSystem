using Shared;
using Shared.Dtos.Category;

namespace ServicesAbstractions
{
    public interface ICategoryService
    {
        Task<ApiResponse<string>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ApiResponse<string>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
        Task<ApiResponse<string>> DeleteCategoryAsync(int categoryId, string deletedBy);
        Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId);
        Task<ApiResponse<PaginatedResponse<CategoryResponseDto>>> GetAllCategoriesAsync(CategorySearchFilterDto filterDto);
    }
}