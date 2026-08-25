using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.Publisher;
using System.Linq.Expressions;

namespace Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLog;

        public PublisherService(IUnitOfWork unitOfWork, IActivityLogService activityLog)
        {
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
        }

        public async Task<ApiResponse<string>> CreatePublisherAsync(CreatePublisherDto dto)
        {
            var nameSpec = new GeneralSpecifications<Publisher>(p => p.Name == dto.Name);
            var existingPublisher = await _unitOfWork.GetRepository<Publisher>().GetAsync(nameSpec);

            if (existingPublisher is not null)
                throw new BadRequestException($"Publisher with name '{dto.Name}' already exists");

            var publisher = new Publisher
            {
                Name = dto.Name,
                Address = dto.Address,
                ContactEmail = dto.ContactEmail,
                Website = dto.Website
            };

            _unitOfWork.GetRepository<Publisher>().Add(publisher);
            await _unitOfWork.SaveChangesAsync();

            // Log the creation of the publisher
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.CreatedBy,
                Action = "Create",
                Details = $"Publisher '{dto.Name}' created with ID {publisher.Id}",
                EntityAffected = nameof(Publisher),
                EntityId = publisher.Id

            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Publisher created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdatePublisherAsync(int publisherId, UpdatePublisherDto dto)
        {
            var spec = new GeneralSpecifications<Publisher>(p => p.Id == publisherId);
            var existingPublisher = await _unitOfWork.GetRepository<Publisher>().GetAsync(spec);

            if (existingPublisher is null) throw new NotFoundException($"Publisher with id {publisherId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != existingPublisher.Name)
            {
                var nameSpec = new GeneralSpecifications<Publisher>(p => p.Name == dto.Name && p.Id != publisherId);
                var nameConflict = await _unitOfWork.GetRepository<Publisher>().GetAsync(nameSpec);
                if (nameConflict is not null) throw new BadRequestException($"Publisher with name '{dto.Name}' already exists");
            }

            existingPublisher.Name = dto.Name ?? existingPublisher.Name;
            existingPublisher.Address = dto.Address ?? existingPublisher.Address;
            existingPublisher.ContactEmail = dto.ContactEmail ?? existingPublisher.ContactEmail;
            existingPublisher.Website = dto.Website ?? existingPublisher.Website;

            _unitOfWork.GetRepository<Publisher>().Update(existingPublisher);
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.UpdatedBy,
                Action = "Update",
                EntityId = existingPublisher.Id,
                EntityAffected = nameof(Publisher),
                Details = $"Publisher '{existingPublisher.Name}' updated"

            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Publisher updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeletePublisherAsync(int publisherId, string deletedBy)
        {
            var spec = new GeneralSpecifications<Publisher>(p => p.Id == publisherId);
            var existingPublisher = await _unitOfWork.GetRepository<Publisher>().GetAsync(spec);

            if (existingPublisher is null) throw new NotFoundException($"Publisher with id {publisherId} not found");

            _unitOfWork.GetRepository<Publisher>().Delete(existingPublisher);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = deletedBy,
                Action = "Delete",
                EntityId = existingPublisher.Id,
                EntityAffected = nameof(Publisher),
                Details = $"Publisher '{existingPublisher.Name}' deleted"

            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Publisher deleted successfully"
            };
        }

        public async Task<ApiResponse<PublisherResponseDto>> GetPublisherByIdAsync(int publisherId)
        {
            Expression<Func<Publisher, PublisherResponseDto>> selector = p => new PublisherResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                ContactEmail = p.ContactEmail,
                Website = p.Website
            };

            var spec = new GeneralSpecifications<Publisher>(p => p.Id == publisherId);
            var publisher = await _unitOfWork.GetRepository<Publisher>().GetAsync(spec, selector);

            if (publisher is null) throw new NotFoundException($"Publisher with id {publisherId} not found");

            return new ApiResponse<PublisherResponseDto>
            {
                Data = publisher,
                Success = true,
                Message = "Publisher retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<PublisherResponseDto>>> GetAllPublishersAsync(PublisherFilterDto filterDto)
        {
            Expression<Func<Publisher, PublisherResponseDto>> selector = p => new PublisherResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                ContactEmail = p.ContactEmail,
                Website = p.Website
            };

            var spec = new GeneralSpecifications<Publisher>(p =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                p.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()),
                filterDto.PageNumber, filterDto.PageSize);

            var publishers = await _unitOfWork.GetRepository<Publisher>().GetAllAsync(spec, selector);

            var countSpec = new GeneralSpecifications<Publisher>(p =>
                string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                p.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()));

            var totalCount = await _unitOfWork.GetRepository<Publisher>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<PublisherResponseDto>>
            {
                Data = new PaginatedResponse<PublisherResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, publishers),
                Success = true,
                Message = "Publishers retrieved successfully"
            };
        }
    }
}