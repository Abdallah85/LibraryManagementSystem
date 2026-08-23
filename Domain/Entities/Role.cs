using Domain.Base;

namespace Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
