using Shared;
using Shared.Dtos.Auth;

namespace ServicesAbstractions;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string rawRefreshToken);
}
