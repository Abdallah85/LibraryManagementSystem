using Domain.Contracts;
using Domain.Entities;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using System.Linq.Expressions;

namespace Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogAsync(CreateActivityLogDto dto)
        {
            var log = new ActivityLog
            {
                UserId = dto.UserId,
                Action = dto.Action,
                EntityAffected = dto.EntityAffected,
                EntityId = dto.EntityId,
                Details = dto.Details,
                Timestamp = DateTime.UtcNow
            };

            _unitOfWork.GetRepository<ActivityLog>().Add(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ApiResponse<PaginatedResponse<ActivityLogResponseDto>>> GetAllLogsAsync(ActivityLogFilterDto filterDto)
        {
            Expression<Func<ActivityLog, bool>> criteria = l =>
                (string.IsNullOrWhiteSpace(filterDto.UserName) || (l.User != null && l.User.UserName.ToLower().Trim().Contains(filterDto.UserName.ToLower().Trim()))) &&
                (string.IsNullOrWhiteSpace(filterDto.UserId) || l.UserId == filterDto.UserId) &&
                (string.IsNullOrWhiteSpace(filterDto.Action) || l.Action.ToLower().Trim().Contains(filterDto.Action.ToLower().Trim())) &&
                (string.IsNullOrWhiteSpace(filterDto.EntityAffected) || l.EntityAffected == filterDto.EntityAffected) &&
                (!filterDto.FromDate.HasValue || l.Timestamp >= filterDto.FromDate) &&
                (!filterDto.ToDate.HasValue || l.Timestamp <= filterDto.ToDate);

            var spec = new GeneralSpecifications<ActivityLog>(criteria, filterDto.PageNumber, filterDto.PageSize);
            var logs = await _unitOfWork.GetRepository<ActivityLog>().GetAllAsync(spec, ActivityLogSelector);

            var countSpec = new GeneralSpecifications<ActivityLog>(criteria);
            var totalCount = await _unitOfWork.GetRepository<ActivityLog>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<ActivityLogResponseDto>>
            {
                Data = new PaginatedResponse<ActivityLogResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, logs),
                Success = true,
                Message = "Activity logs retrieved successfully"
            };
        }

        private static readonly Expression<Func<ActivityLog, ActivityLogResponseDto>> ActivityLogSelector = l => new ActivityLogResponseDto
        {
            Id = l.Id,
            UserId = l.UserId,
            UserName = l.User != null ? l.User.UserName : null,
            Action = l.Action,
            EntityAffected = l.EntityAffected,
            EntityId = l.EntityId,
            Timestamp = l.Timestamp,
            Details = l.Details
        };
    }
}