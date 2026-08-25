

namespace Shared.Dtos.User
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsMember { get; set; }
        public string? Role { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
