
using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Book : BaseEntity
{
    public string ISBN { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Edition { get; set; }
    public string? Summary { get; set; }
    public int PublicationYear { get; set; }
    public string? CoverImageUrl { get; set; }
    public BookStatus Status { get; set; } = BookStatus.InLibrary;

    public int LanguageId { get; set; }
    public Language Language { get; set; } = default!;

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = default!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
}
