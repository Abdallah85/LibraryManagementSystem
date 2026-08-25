using Shared;
using Shared.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesAbstractions
{
    public interface IUserService
    {
        Task<ApiResponse<string>> CreateUserAsync(CreateUserDto dto);
        Task<ApiResponse<string>> UpdateUserAsync(string userId, UpdateUserDto dto);
        Task<ApiResponse<string>> DeleteUserAsync(string userId, string deletedBy);
        Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<PaginatedResponse<UserResponseDto>>> GetAllUsersAsync(UserFilterDto filterDto);
    }
}
