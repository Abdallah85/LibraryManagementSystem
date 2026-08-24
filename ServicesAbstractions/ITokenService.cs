

namespace ServicesAbstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken( user, IList<string> roles);

    /// <summary>Cryptographically random, high-entropy raw refresh token value.</summary>
    string GenerateRefreshTokenValue();

    /// <summary>SHA-256 hash of a raw token, for DB storage/lookup.</summary>
    string HashToken(string rawToken);
}
