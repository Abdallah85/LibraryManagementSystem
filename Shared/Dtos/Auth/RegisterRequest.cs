using System.ComponentModel.DataAnnotations;


namespace Shared.Dtos.Auth
{
    public class RegisterRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Username { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(8)]
        public string Password { get; set; } = default!;

    }
}
