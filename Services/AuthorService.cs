using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.Author;
using System.Linq.Expressions;

namespace Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLog;

        public AuthorService(IUnitOfWork unitOfWork, IActivityLogService activityLog)
        {
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
        }

        public async Task<ApiResponse<string>> CreateAuthorAsync(CreateAuthorDto dto)
        {
            var author = new Author
            {
                FullName = dto.FullName,
                Bio = dto.Bio
            };

            _unitOfWork.GetRepository<Author>().Add(author);
            await _unitOfWork.SaveChangesAsync();

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.CreatedBy,
                Action = "Create",
                EntityAffected = nameof(Author),
                EntityId = author.Id,
                Details = $"Author '{author.FullName}' created."
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Author created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdateAuthorAsync(int authorId, UpdateAuthorDto dto)
        {
            var spec = new GeneralSpecifications<Author>(a => a.Id == authorId);
            var existingAuthor = await _unitOfWork.GetRepository<Author>().GetAsync(spec);

            if (existingAuthor is null) throw new NotFoundException($"Author with id {authorId} not found");

            existingAuthor.FullName = dto.FullName ?? existingAuthor.FullName;
            existingAuthor.Bio = dto.Bio ?? existingAuthor.Bio;

            _unitOfWork.GetRepository<Author>().Update(existingAuthor);
            await _unitOfWork.SaveChangesAsync();

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.UpdatedBy,
                Action = "Update",
                EntityAffected = nameof(Author),
                EntityId = authorId,
                Details = $"Author '{existingAuthor.FullName}' updated."
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Author updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeleteAuthorAsync(int authorId, string deletedBy)
        {
            var spec = new GeneralSpecifications<Author>(a => a.Id == authorId);
            var existingAuthor = await _unitOfWork.GetRepository<Author>().GetAsync(spec);

            if (existingAuthor is null) throw new NotFoundException($"Author with id {authorId} not found");

            var bookAuthorSpec = new GeneralSpecifications<BookAuthor>(ba => ba.AuthorId == authorId);
            var linkedBooksCount = await _unitOfWork.GetRepository<BookAuthor>().CountAsync(bookAuthorSpec);

            if (linkedBooksCount > 0)
                throw new BadRequestException($"Cannot delete author '{existingAuthor.FullName}' because they are linked to existing books");

            _unitOfWork.GetRepository<Author>().Delete(existingAuthor);
            await _unitOfWork.SaveChangesAsync();

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = deletedBy,
                Action = "Delete",
                EntityAffected = nameof(Author),
                EntityId = authorId,
                Details = $"Author '{existingAuthor.FullName}' deleted."
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Author deleted successfully"
            };
        }

        public async Task<ApiResponse<AuthorResponseDto>> GetAuthorByIdAsync(int authorId)
        {
            var spec = new GeneralSpecifications<Author>(a => a.Id == authorId);
            var author = await _unitOfWork.GetRepository<Author>().GetAsync(spec, AuthorSelector);

            if (author is null) throw new NotFoundException($"Author with id {authorId} not found");

            return new ApiResponse<AuthorResponseDto>
            {
                Data = author,
                Success = true,
                Message = "Author retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<AuthorResponseDto>>> GetAllAuthorsAsync(AuthorSearchFilterDto filterDto)
        {
            Expression<Func<Author, bool>> criteria = a =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                a.FullName.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim());

            var spec = new GeneralSpecifications<Author>(criteria, filterDto.PageNumber, filterDto.PageSize);
            var authors = await _unitOfWork.GetRepository<Author>().GetAllAsync(spec, AuthorSelector);

            var countSpec = new GeneralSpecifications<Author>(criteria);
            var totalCount = await _unitOfWork.GetRepository<Author>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<AuthorResponseDto>>
            {
                Data = new PaginatedResponse<AuthorResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, authors),
                Success = true,
                Message = "Authors retrieved successfully"
            };
        }

        private static readonly Expression<Func<Author, AuthorResponseDto>> AuthorSelector = a => new AuthorResponseDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Bio = a.Bio
        };
    }
}