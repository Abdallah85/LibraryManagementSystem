using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.User;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IActivityLogService _activityLog;

        public UserService(UserManager<User> userManager, IActivityLogService activityLog)
        {
            _userManager = userManager;
            _activityLog = activityLog;
        }

        public async Task<ApiResponse<string>> CreateUserAsync(CreateUserDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                throw new ConflictException("Email is already in use.");

            if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                throw new ConflictException("Username is already in use.");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                IsMember = dto.IsMember,
                MembershipDate = dto.IsMember ? DateTime.UtcNow : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.CreatedBy,
                Action = "UserCreated",
                EntityAffected = nameof(User),
                Details = $"User '{user.UserName}' created with role '{dto.Role}' (id: {user.Id})"
            });

            return new ApiResponse<string>
            {
                Data = user.Id,
                Success = true,
                Message = "User created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new NotFoundException($"User with id {userId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                    throw new ConflictException("Email is already in use.");
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
            {
                if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                    throw new ConflictException("Username is already in use.");
                user.UserName = dto.UserName;
            }

            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
            if (dto.IsMember.HasValue) user.IsMember = dto.IsMember.Value;

            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = dto.UpdatedBy,
                Action = "UserUpdated",
                EntityAffected = nameof(User),
                Details = $"User '{user.UserName}' (id: {userId}) updated"
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "User updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(string userId, string deletedBy)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new NotFoundException($"User with id {userId} not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = deletedBy,
                Action = "UserDeactivated",
                EntityAffected = nameof(User),
                Details = $"User '{user.UserName}' (id: {userId}) deactivated"
            });

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "User deactivated successfully"
            };
        }

        public async Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new NotFoundException($"User with id {userId} not found");

            var roles = await _userManager.GetRolesAsync(user);

            return new ApiResponse<UserResponseDto>
            {
                Data = MapToDto(user, roles),
                Success = true,
                Message = "User retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<UserResponseDto>>> GetAllUsersAsync(UserFilterDto filterDto)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
            {
                var term = filterDto.SearchTerm.ToLower().Trim();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)));
            }

            if (filterDto.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filterDto.IsActive.Value);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .ToListAsync();

            var results = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrWhiteSpace(filterDto.Role) && !roles.Contains(filterDto.Role))
                    continue;

                results.Add(MapToDto(user, roles));
            }

            return new ApiResponse<PaginatedResponse<UserResponseDto>>
            {
                Data = new PaginatedResponse<UserResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, results),
                Success = true,
                Message = "Users retrieved successfully"
            };
        }

        private static UserResponseDto MapToDto(User user, IList<string> roles) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsMember = user.IsMember,
            MembershipDate = user.MembershipDate,
            Status = user.Status?.ToString(),
            Roles = roles
        };
    }
}