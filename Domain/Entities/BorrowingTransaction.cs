using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public class BorrowingTransaction : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public string UserId { get; set; } =null!;
    public User User { get; set; } = default!;

    public string? IssuedByUserId { get; set; } 
    public User? IssuedByUser { get; set; } 

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowingStatus Status { get; set; } = BorrowingStatus.Borrowed;
    public decimal? FineAmount { get; set; }
}
