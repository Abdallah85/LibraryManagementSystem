namespace Shared.Dtos.ActivityLog
{
    public class ActivityLogFilterDto
    {
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? Action { get; set; }
        public string? EntityAffected { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
