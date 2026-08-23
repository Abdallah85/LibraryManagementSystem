namespace Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedAt { get; set; }
        public string? ReasonRevoked { get; set; }

        public bool IsExpired { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsActive { get; set; }
    }
}
