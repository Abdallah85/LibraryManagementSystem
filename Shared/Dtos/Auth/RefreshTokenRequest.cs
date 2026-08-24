using System.ComponentModel.DataAnnotations;


namespace Shared.Dtos.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
