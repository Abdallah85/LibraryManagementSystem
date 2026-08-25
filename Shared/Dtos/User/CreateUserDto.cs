

namespace Shared.Dtos.User
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Role { get; set; } = "Member";
        public bool IsMember { get; set; } = true;
        public string? CreatedBy { get; set; }
    }
}
