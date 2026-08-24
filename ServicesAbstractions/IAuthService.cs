using LibrarySystem.Application.Common;
using LibrarySystem.Application.DTOs.Auth;

namespace ServicesAbstractions;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequest request, string? ipAddress);
    Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string rawRefreshToken, string? ipAddress);
    Task<ServiceResult<bool>> RevokeTokenAsync(string rawRefreshToken, string? ipAddress);
}
