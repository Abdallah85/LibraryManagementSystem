namespace Shared.Dtos.ActivityLog
{
    public class ActivityLogResponseDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = default!;
        public string? EntityAffected { get; set; }
        public int? EntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
    }
}
