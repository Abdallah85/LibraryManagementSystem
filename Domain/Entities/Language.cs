using Domain.Base;


namespace Domain.Entities;

public class Language : BaseEntity
{
    public string Name { get; set; } = default!;   // e.g. "English"
    public string Code { get; set; } = default!;   // e.g. "en", "ar"

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
