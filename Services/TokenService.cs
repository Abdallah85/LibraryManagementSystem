namespace Services
{
    using Domain.Entities;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using ServicesAbstractions;
    using Shared.Config;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    namespace Services
    {
        public class TokenService : ITokenService
        {
            private readonly JwtSettings _jwtSettings;

            public TokenService(IOptions<JwtSettings> jwtOptions)
            {
                _jwtSettings = jwtOptions.Value;
            }

            public (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, IList<string> roles)
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

                var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return (tokenString, expiresAt);
            }

            public string GenerateRefreshTokenValue()
            {
                var randomBytes = RandomNumberGenerator.GetBytes(64);
                return Convert.ToBase64String(randomBytes);
            }

            public string HashToken(string rawToken)
            {
                var bytes = Encoding.UTF8.GetBytes(rawToken);
                var hash = SHA256.HashData(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
