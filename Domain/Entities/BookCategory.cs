namespace Domain.Entities;

public class BookCategory
{
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}
