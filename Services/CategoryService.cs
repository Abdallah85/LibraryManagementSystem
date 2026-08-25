using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.Category;
using System.Linq.Expressions;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLog;

        public CategoryService(IUnitOfWork unitOfWork, IActivityLogService activityLog)
        {
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
        }

        public async Task<ApiResponse<string>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var nameSpec = new GeneralSpecifications<Category>(c => c.Name == dto.Name);
            var existingCategory = await _unitOfWork.GetRepository<Category>().GetAsync(nameSpec);

            if (existingCategory is not null)
                throw new BadRequestException($"Category with name '{dto.Name}' already exists");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _unitOfWork.GetRepository<Category>().Add(category);
            await _unitOfWork.SaveChangesAsync();

            // Log the activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                EntityId = category.Id,
                EntityAffected = nameof(Category),
                Details = $"Category '{category.Name}' created",
                Action = "Create",
                UserId = dto.CreatedBy
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Category created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            var spec = new GeneralSpecifications<Category>(c => c.Id == categoryId);
            var existingCategory = await _unitOfWork.GetRepository<Category>().GetAsync(spec);

            if (existingCategory is null) throw new NotFoundException($"Category with id {categoryId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != existingCategory.Name)
            {
                var nameSpec = new GeneralSpecifications<Category>(c => c.Name == dto.Name && c.Id != categoryId);
                var nameConflict = await _unitOfWork.GetRepository<Category>().GetAsync(nameSpec);
                if (nameConflict is not null) throw new BadRequestException($"Category with name '{dto.Name}' already exists");
            }

            existingCategory.Name = dto.Name ?? existingCategory.Name;
            existingCategory.Description = dto.Description ?? existingCategory.Description;

            _unitOfWork.GetRepository<Category>().Update(existingCategory);

            // Log the activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                EntityId = existingCategory.Id,
                EntityAffected = nameof(Category),
                Details = $"Category '{existingCategory.Name}' updated",
                Action = "Update",
                UserId = dto.UpdatedBy
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Category updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeleteCategoryAsync(int categoryId, string deletedBy)
        {
            var spec = new GeneralSpecifications<Category>(c => c.Id == categoryId);
            var existingCategory = await _unitOfWork.GetRepository<Category>().GetAsync(spec);

            if (existingCategory is null) throw new NotFoundException($"Category with id {categoryId} not found");

            _unitOfWork.GetRepository<Category>().Delete(existingCategory);

            // Log the activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                EntityId = existingCategory.Id,
                EntityAffected = nameof(Category),
                Details = $"Category '{existingCategory.Name}' deleted",
                Action = "Delete",
                UserId = deletedBy
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Category deleted successfully"
            };
        }

        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            Expression<Func<Category, CategoryResponseDto>> selector = c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            };

            var spec = new GeneralSpecifications<Category>(c => c.Id == categoryId);
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(spec, selector);

            if (category is null) throw new NotFoundException($"Category with id {categoryId} not found");

            return new ApiResponse<CategoryResponseDto>
            {
                Data = category,
                Success = true,
                Message = "Category retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<CategoryResponseDto>>> GetAllCategoriesAsync(CategorySearchFilterDto filterDto)
        {
            Expression<Func<Category, CategoryResponseDto>> selector = c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            };

            var spec = new GeneralSpecifications<Category>(c =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                c.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()),
                filterDto.PageNumber, filterDto.PageSize);

            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(spec, selector);

            var countSpec = new GeneralSpecifications<Category>(c =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                c.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()));

            var totalCount = await _unitOfWork.GetRepository<Category>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<CategoryResponseDto>>
            {
                Data = new PaginatedResponse<CategoryResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, categories),
                Success = true,
                Message = "Categories retrieved successfully"
            };
        }
    }
}