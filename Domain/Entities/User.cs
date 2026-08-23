using Domain.Base;


namespace Domain.Entities;
public class User : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public ICollection<BorrowingTransaction> IssuedTransactions { get; set; } = new List<BorrowingTransaction>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}
