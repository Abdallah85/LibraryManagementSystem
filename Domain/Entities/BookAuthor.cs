

namespace Domain.Entities;

public class BookAuthor
{
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public int AuthorId { get; set; }
    public Author Author { get; set; } = default!;
}
