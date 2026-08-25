using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.Language;
using System.Linq.Expressions;

namespace Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLog;

        public LanguageService(IUnitOfWork unitOfWork, IActivityLogService activityLog)
        {
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
        }

        public async Task<ApiResponse<string>> CreateLanguageAsync(CreateLanguageDto dto)
        {
            var duplicateSpec = new GeneralSpecifications<Language>(l =>
                l.Name == dto.Name || l.Code == dto.Code);
            var existing = await _unitOfWork.GetRepository<Language>().GetAllAsync(duplicateSpec);

            if (existing.Any(l => l.Name == dto.Name))
                throw new BadRequestException($"Language with name '{dto.Name}' already exists");

            if (existing.Any(l => l.Code == dto.Code))
                throw new BadRequestException($"Language with code '{dto.Code}' already exists");

            var language = new Language
            {
                Name = dto.Name,
                Code = dto.Code
            };

            _unitOfWork.GetRepository<Language>().Add(language);
            await _unitOfWork.SaveChangesAsync();

            //Log the creation of the language
            await _activityLog.LogAsync(new Shared.Dtos.ActivityLog.CreateActivityLogDto
            {
                UserId = dto.CreatedBy,
                Action = "Create",
                Details = $"Language '{language.Name}' with code '{language.Code}' created.",
                EntityAffected = nameof(Language),
                EntityId = language.Id
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Language created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdateLanguageAsync(int languageId, UpdateLanguageDto dto)
        {
            var spec = new GeneralSpecifications<Language>(l => l.Id == languageId);
            var existingLanguage = await _unitOfWork.GetRepository<Language>().GetAsync(spec);

            if (existingLanguage is null) throw new NotFoundException($"Language with id {languageId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != existingLanguage.Name)
            {
                var nameSpec = new GeneralSpecifications<Language>(l => l.Name == dto.Name && l.Id != languageId);
                var nameConflict = await _unitOfWork.GetRepository<Language>().GetAsync(nameSpec);
                if (nameConflict is not null) throw new BadRequestException($"Language with name '{dto.Name}' already exists");
            }

            if (!string.IsNullOrWhiteSpace(dto.Code) && dto.Code != existingLanguage.Code)
            {
                var codeSpec = new GeneralSpecifications<Language>(l => l.Code == dto.Code && l.Id != languageId);
                var codeConflict = await _unitOfWork.GetRepository<Language>().GetAsync(codeSpec);
                if (codeConflict is not null) throw new BadRequestException($"Language with code '{dto.Code}' already exists");
            }

            existingLanguage.Name = dto.Name ?? existingLanguage.Name;
            existingLanguage.Code = dto.Code ?? existingLanguage.Code;

            _unitOfWork.GetRepository<Language>().Update(existingLanguage);


            //Log the update of the language
            await _activityLog.LogAsync(new Shared.Dtos.ActivityLog.CreateActivityLogDto
            {
                UserId = dto.UpdatedBy,
                Action = "Update",
                Details = $"Language '{existingLanguage.Name}' with code '{existingLanguage.Code}' updated.",
                EntityAffected = nameof(Language),
                EntityId = existingLanguage.Id
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Language updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeleteLanguageAsync(int languageId, string? deletedBy)
        {
            var spec = new GeneralSpecifications<Language>(l => l.Id == languageId);
            var existingLanguage = await _unitOfWork.GetRepository<Language>().GetAsync(spec);

            if (existingLanguage is null) throw new NotFoundException($"Language with id {languageId} not found");

            _unitOfWork.GetRepository<Language>().Delete(existingLanguage);
            await _unitOfWork.SaveChangesAsync();

            //Log the deletion of the language
            await _activityLog.LogAsync(new Shared.Dtos.ActivityLog.CreateActivityLogDto
            {
                UserId = deletedBy,
                Action = "Delete",
                Details = $"Language '{existingLanguage.Name}' with code '{existingLanguage.Code}' deleted.",
                EntityAffected = nameof(Language),
                EntityId = existingLanguage.Id
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Language deleted successfully"
            };
        }

        public async Task<ApiResponse<LanguageResponseDto>> GetLanguageByIdAsync(int languageId)
        {
            Expression<Func<Language, LanguageResponseDto>> selector = l => new LanguageResponseDto
            {
                Id = l.Id,
                Name = l.Name,
                Code = l.Code
            };

            var spec = new GeneralSpecifications<Language>(l => l.Id == languageId);
            var language = await _unitOfWork.GetRepository<Language>().GetAsync(spec, selector);

            if (language is null) throw new NotFoundException($"Language with id {languageId} not found");

            return new ApiResponse<LanguageResponseDto>
            {
                Data = language,
                Success = true,
                Message = "Language retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<LanguageResponseDto>>> GetAllLanguagesAsync(LanguageSearchFilterDto filterDto)
        {
            Expression<Func<Language, LanguageResponseDto>> selector = l => new LanguageResponseDto
            {
                Id = l.Id,
                Name = l.Name,
                Code = l.Code
            };

            var spec = new GeneralSpecifications<Language>(l =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                l.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()) ||
                l.Code.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()),
                filterDto.PaegeNumber, filterDto.PageSize);

            var languages = await _unitOfWork.GetRepository<Language>().GetAllAsync(spec, selector);

            var countSpec = new GeneralSpecifications<Language>(l =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                l.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()) ||
                l.Code.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()));

            var totalCount = await _unitOfWork.GetRepository<Language>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<LanguageResponseDto>>
            {
                Data = new PaginatedResponse<LanguageResponseDto>(filterDto.PaegeNumber, filterDto.PageSize, totalCount, languages),
                Success = true,
                Message = "Languages retrieved successfully"
            };
        }
    }
}