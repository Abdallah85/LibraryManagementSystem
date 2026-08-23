using Domain.Base;


namespace Domain.Entities;

public class Publisher : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
