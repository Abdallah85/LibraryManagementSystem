using Shared.Enums;

namespace Shared.Dtos.Borrowing;

public class BorrowingTransactionResponseDto
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public string? IssuedByUserId { get; set; }
    public string? IssuedByUserName { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public BorrowingStatus Status { get; set; } 

}


public class BorrowingTransactionFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? UserId { get; set; }
    public int? BookId { get; set; }
    public BorrowingStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
}