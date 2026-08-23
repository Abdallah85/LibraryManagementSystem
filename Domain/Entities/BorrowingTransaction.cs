using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public class BorrowingTransaction : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public int IssuedByUserId { get; set; }
    public User IssuedByUser { get; set; } = default!;

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowingStatus Status { get; set; } = BorrowingStatus.Borrowed;
    public decimal? FineAmount { get; set; }
}
