using Domain.Base;

namespace Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}
