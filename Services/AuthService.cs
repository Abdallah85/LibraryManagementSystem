using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServicesAbstractions;
using Shared;
using Shared.Config;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.Auth;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IActivityLogService _activityLog;

        private const string DefaultRole = "Member";

        public AuthService(
            UserManager<User> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IActivityLogService activityLog,
            IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                throw new ConflictException("Email is already in use.");

            if (await _userManager.FindByNameAsync(request.Username) is not null)
                throw new ConflictException("Username is already in use.");

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                IsMember = true,
                MembershipDate = DateTime.UtcNow,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            var authResponse = await BuildAuthResponseAsync(user);

            // Log the registration activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = user.Id,
                Action = "UserRegistered",
                EntityAffected = nameof(User),
                EntityId = null,
                Details = $"New user '{user.UserName}' registered with email '{user.Email}'"
            });

            return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Registration successful.");
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UsernameOrEmail)
                       ?? await _userManager.FindByNameAsync(request.UsernameOrEmail);

            if (user is null)
                throw new UnauthorizedException("Invalid credentials.");

            if (!user.IsActive)
                throw new UnauthorizedException("This account has been deactivated.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedException("Invalid credentials.");

            var authResponse = await BuildAuthResponseAsync(user);

            // Log the login activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = user.Id,
                Action = "UserLoggedIn",
                EntityAffected = nameof(User),
                EntityId = null,
                Details = $"User '{user.UserName}' logged in."
            });

            return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful.");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string rawRefreshToken)
        {
            var tokenHash = _tokenService.HashToken(rawRefreshToken);
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, string>();

            var storedToken = await refreshTokenRepo.Query()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (storedToken is null || !storedToken.IsActive)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            if (!storedToken.User.IsActive)
                throw new UnauthorizedException("This account has been deactivated.");

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.IsActive = false;
            storedToken.IsRevoked = true;
            refreshTokenRepo.Update(storedToken);

            var authResponse = await BuildAuthResponseAsync(storedToken.User);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = storedToken.UserId,
                Action = "TokenRefreshed",
                EntityAffected = nameof(User),
                EntityId = null,
                Details = $"User '{storedToken.User.UserName}' refreshed their token."
            });

            return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Token refreshed.");
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, accessExpiresAt) = _tokenService.GenerateAccessToken(user, roles);

            var rawRefreshToken = _tokenService.GenerateRefreshTokenValue();
            var refreshTokenHash = _tokenService.HashToken(rawRefreshToken);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, string>();


            refreshTokenRepo.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = refreshExpiresAt,
                IsActive = true,
                IsExpired = false,
                IsRevoked = false,
            });

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiresAt,
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt,
                UserId = user.Id,
                Username = user.UserName!,
                Role = roles.FirstOrDefault() ?? DefaultRole
            };
        }
    }
}