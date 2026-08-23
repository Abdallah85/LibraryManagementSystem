using Domain.Base;
using Domain.Enums;


namespace Domain.Entities;

public class Member : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;

    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
}
