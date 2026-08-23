using Domain.Base;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;


namespace Domain.Entities;

public class User : IdentityUser<string>
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsMember { get; set; } = false;
    public DateTime? MembershipDate { get; set; }
    public MembershipStatus? Status { get; set; }

    public ICollection<BorrowingTransaction> IssuedTransactions { get; set; } = new List<BorrowingTransaction>();
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
