namespace Shared.Dtos.ActivityLog
{
    public class CreateActivityLogDto
    {
        public string? UserId { get; set; }
        public string Action { get; set; } = default!;
        public string? EntityAffected { get; set; }
        public int? EntityId { get; set; }
        public string? Details { get; set; }
    }
}
