using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Language
{
    public class CreateLanguageDto
    {
        [Required(ErrorMessage = "Language name is required.")]
        [MaxLength(50, ErrorMessage = "Language name must not exceed 50 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Language code is required.")]
        [MaxLength(5, ErrorMessage = "Language code must not exceed 5 characters.")]
        [RegularExpression(@"^[a-zA-Z\-]+$", ErrorMessage = "Language code must contain only letters and hyphens (e.g., 'en', 'en-US').")]
        public string Code { get; set; } = default!;

        public string? CreatedBy { get; set; }
    }
}