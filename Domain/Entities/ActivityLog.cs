using Domain.Base;

namespace Domain.Entities;

public class ActivityLog : BaseEntity
{
    public int? UserId { get; set; }
    public User? User { get; set; }

    public string Action { get; set; } = default!;         
    public string? EntityAffected { get; set; }             
    public int? EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }    
}
