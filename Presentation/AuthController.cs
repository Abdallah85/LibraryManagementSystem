using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.Dtos.Auth;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _AuthService;

        public AuthController(IAuthService authService)
        {
            _AuthService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _AuthService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _AuthService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _AuthService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }
    }
}