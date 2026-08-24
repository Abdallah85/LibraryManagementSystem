using Shared;
using Shared.Dtos.Publisher;

namespace ServicesAbstractions
{
    public interface IPublisherService
    {
        Task<ApiResponse<string>> CreatePublisherAsync(CreatePublisherDto dto);
        Task<ApiResponse<string>> UpdatePublisherAsync(int publisherId, UpdatePublisherDto dto);
        Task<ApiResponse<string>> DeletePublisherAsync(int publisherId);
        Task<ApiResponse<PublisherResponseDto>> GetPublisherByIdAsync(int publisherId);
        Task<ApiResponse<PaginatedResponse<PublisherResponseDto>>> GetAllPublishersAsync(PublisherFilterDto filterDto);
    }
}