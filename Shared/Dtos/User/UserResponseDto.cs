

namespace Shared.Dtos.User
{
    public class UserResponseDto
    {
        public string Id { get; set; } = default!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsMember { get; set; }
        public DateTime? MembershipDate { get; set; }
        public string? Status { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
