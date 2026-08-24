namespace Shared.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public DateTime AccessTokenExpiresAt { get; set; }

        public string RefreshToken { get; set; } = default!;
        public DateTime RefreshTokenExpiresAt { get; set; }

        public string UserId { get; set; }
        public string Username { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
