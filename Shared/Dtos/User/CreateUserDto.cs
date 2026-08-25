using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(256, ErrorMessage = "Username must not exceed 256 characters.")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = "Member";

        public bool IsMember { get; set; } = true;

        public string? CreatedBy { get; set; }
    }
}