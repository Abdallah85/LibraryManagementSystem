using Domain.Base;

namespace Domain.Entities;

public class Author : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string? Bio { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
