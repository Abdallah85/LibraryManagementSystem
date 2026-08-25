using Shared;
using Shared.Dtos.ActivityLog;

namespace ServicesAbstractions
{
    public interface IActivityLogService
    {
        Task LogAsync(CreateActivityLogDto dto);
        Task<ApiResponse<PaginatedResponse<ActivityLogResponseDto>>> GetAllLogsAsync(ActivityLogFilterDto filterDto);
    }
}
