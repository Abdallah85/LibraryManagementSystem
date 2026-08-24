using Domain.Entities;

namespace ServicesAbstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, IList<string> roles);

    string GenerateRefreshTokenValue();

    string HashToken(string rawToken);
}
